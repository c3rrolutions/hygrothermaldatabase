#!/usr/bin/env -S make --file
SELF := $(lastword $(MAKEFILE_LIST))

include ./.env

SHELL := /usr/bin/env bash
.SHELLFLAGS := -o errexit -o errtrace -o nounset -o pipefail -c
MAKEFLAGS += --warn-undefined-variables

COMPOSE_BAKE=true

dump_archive_name = postgresql_dumpall.gz
files_archive_name = files.gz

# Taken from https://www.client9.com/self-documenting-makefiles/
help : ## Print this help
	@awk -F ':|##' '/^[^\t].+?:.*?##/ {\
		printf "\033[36m%-30s\033[0m %s\n", $$1, $$NF \
	}' $(MAKEFILE_LIST)
.PHONY : help
.DEFAULT_GOAL := help

psql : ## Enter PostgreSQL interactive terminal in the `database` container
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		database \
		psql \
		--username="${POSTGRES_USER}" \
		--dbname="${POSTGRES_DATABASE_NAME}"
.PHONY : psql

remove-volume : ## Remove data and files volumes
	docker volume rm \
		"${NAME}_${ENVIRONMENT}_data"
	docker volume rm \
		"${NAME}_${ENVIRONMENT}_files"
.PHONY : remove-volume

create : ## Create database with name `${POSTGRES_DATABASE_NAME}`
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		--no-tty \
		database \
		createdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
.PHONY : create

drop : ## Drop database with name `${POSTGRES_DATABASE_NAME}`
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		--no-tty \
		database \
		dropdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
.PHONY : drop

sql : ## Run the SQL script in the file `${SCRIPT}` in the database service, for example, `make sql SCRIPT=./my.sql ` (note that after database schema changes it is necessary to restart the backend service for the object-relational mapper Npgsql to work seamlessly, for example, by restarting the backend service with `./docker.mk restart SERVICE=backend`)
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	cat "${SCRIPT}" \
	| docker compose exec \
		--no-tty \
		database \
		psql \
			--echo-all \
			--no-psqlrc \
			--set=ON_ERROR_STOP=on \
			--file=- \
			--username="${POSTGRES_USER}" \
			--dbname="${POSTGRES_DATABASE_NAME}"
.PHONY : sql

migrate : SCRIPT = ./backend/src/Migrations/migrate.sql
migrate : ## Migrate database  by running the idempotent SQL script ./backend/src/Migrations/migrate.sql
	$(MAKE) --file="${SELF}" sql SCRIPT="${SCRIPT}"
	docker compose restart \
		--no-deps \
		backend
.PHONY : migrate

# Backup with `pg_dump`: https://www.postgresql.org/docs/current/backup-dump.html
# Command `pg_dump`: https://www.postgresql.org/docs/current/app-pgdump.html
# Backup files with `tar` and `gzip` as suggested in https://docs.docker.com/storage/volumes/#backup-restore-or-migrate-data-volumes
# We could have used `docker cp` as explained in https://docs.docker.com/engine/reference/commandline/cp/
backup : DIR = ./backup
backup : ## Backup database and related data to directory with absolute path `${DIR}`, for example, `./database.mk backup DIR=/app/data/backups/$(date +"%Y-%m-%d_%H_%M_%S")`
	mkdir --parents "${DIR}"
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	docker compose exec \
		--no-tty \
		database \
		pg_dump \
			--clean \
			--if-exists \
			--username="${POSTGRES_USER}" \
			--dbname="${POSTGRES_DATABASE_NAME}" \
		| gzip \
		> "${DIR}/${dump_archive_name}"
	docker compose run \
		--rm \
		--no-deps \
		--no-TTY \
		--volume "${DIR}":/backup \
		backend \
		tar \
			--verbose \
			--create \
			--gzip \
			--file="/backup/${files_archive_name}" \
			--directory=/app \
			./files
.PHONY : backup

restore : DIR = ./backup
restore : ## Restore database and related data from directory with absolute path `${DIR}` (dropping and recreating the database and clearing related files before to start cleanly), for example, `./database.mk restore DIR=/app/data/backups/2021-04-22_15_43_35`
	docker compose stop \
		backend
	docker compose up \
		--no-build \
		--no-recreate \
		--wait \
		database
	-docker compose exec \
		--no-tty \
		database \
		dropdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
	docker compose exec \
		--no-tty \
		database \
		createdb \
			--username="${POSTGRES_USER}" \
			"${POSTGRES_DATABASE_NAME}"
	gunzip --stdout "${DIR}/${dump_archive_name}" \
	| docker compose exec \
		--no-tty \
		database \
		psql \
			--echo-all \
			--no-psqlrc \
			--set=ON_ERROR_STOP=on \
			--file=- \
			--username="${POSTGRES_USER}" \
			--dbname="${POSTGRES_DATABASE_NAME}"
	docker compose run \
		--rm \
		--no-deps \
		--no-TTY \
		--volume "${DIR}":/backup \
		backend \
		bash -o errexit -o errtrace -o nounset -o pipefail -c " \
			if [[ "${ENVIRONMENT}" == "development" ]]; then \
				cd /app/src/files ; \
			else \
				cd /app/files ; \
			fi \
			&& rm \
				--recursive \
				--force \
				--dir \
				* \
			&& tar \
				--verbose \
				--extract \
				--gunzip \
				--strip-components=2 \
				--file='/backup/${files_archive_name}' \
		"
	docker compose start \
		backend
.PHONY : restore
