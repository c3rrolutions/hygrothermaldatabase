#!/usr/bin/env -S make --file
SELF := $(lastword $(MAKEFILE_LIST))

SHELL := /usr/bin/env bash
.SHELLFLAGS := -o errexit -o errtrace -o nounset -o pipefail -c
MAKEFLAGS += --warn-undefined-variables

NAME = database
TARGET = $(shell git rev-parse --verify HEAD)

# Taken from https://www.client9.com/self-documenting-makefiles/
help : ## Print this help
	@awk -F ':|##' '/^[^\t].+?:.*?##/ {\
		printf "\033[36m%-30s\033[0m %s\n", $$1, $$NF \
	}' $(MAKEFILE_LIST)
.PHONY : help
.DEFAULT_GOAL := help

target : ## Print value of variable `TARGET`
	@echo ${TARGET}
.PHONY : target

build : OVERWRITE = false
build : ## Build image if it does not exist yet, for example, `./forge.mk build SERVICE=backend` or even if it does exist `./forge.mk build SERVICE=backend OVERWRITE=true`
	if [[ "${OVERWRITE}" == "true" || -z "$(shell docker images --quiet '${NAME}-${SERVICE}:${TARGET}')" ]]; then \
		docker build \
			--pull \
			--file "./${SERVICE}/Dockerfile.production" \
			--tag "${NAME}-${SERVICE}:${TARGET}" \
			"./${SERVICE}" ; \
	else \
		echo "Image already exists, skipping build. Pass `OVERWRITE=true` to build anyway." ; \
	fi
.PHONY : build

push : ## Push image, for example, `./forge.mk push SERVICE=backend USER=cloud HOST=192.102.162.39`
	docker save \
		--platform "linux/amd64" \
		"${NAME}-${SERVICE}:${TARGET}" \
		| bzip2 \
		| ssh "${USER}@${HOST}" "docker load"
.PHONY : push

all : ## Build and push all images, for example, `./forge.mk all USER=cloud HOST=192.102.162.39`
	$(MAKE) --file="${SELF}" \
		build \
		push \
		SERVICE=backend
	$(MAKE) --file="${SELF}" \
		build \
		push \
		SERVICE=frontend
.PHONY : all

# deploy : all ## Deploy in the cloud, for example, `./forge.mk deploy ENVIRONMENT=staging USER=cloud HOST=192.102.162.39`
# 	ssh "${USER}@${HOST}" " \
# 		cd '/app/${ENVIRONMENT}' \
# 		&& ./deploy.mk deploy TARGET='${TARGET}' \
# 	"
# .PHONY : deploy
