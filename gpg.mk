#!/usr/bin/env -S make --file

include ./.env

SHELL := /usr/bin/env bash
.SHELLFLAGS := -o errexit -o errtrace -o nounset -o pipefail -c
MAKEFLAGS += --warn-undefined-variables

# Taken from https://www.client9.com/self-documenting-makefiles/
help : ## Print this help
	@awk -F ':|##' '/^[^\t].+?:.*?##/ {\
		printf "\033[36m%-30s\033[0m %s\n", $$1, $$NF \
	}' $(MAKEFILE_LIST)
.PHONY : help
.DEFAULT_GOAL := help

# Keep file name <FINGERPRINT>.gpg in sync with the one in `AppSettings.cs`
key : COMMENT =
key : build-bootstrap ## Generate GnuPG key with the passphrase `${GNUPG_SECRET_SIGNING_KEY_PASSPHRASE}`, for example, `make gpg PERSON="Simon Wacker" COMMENT=solarbuildingenvelopes EMAIL=simon.wacker@ise.fraunhofer.de`
	docker run \
		--rm \
		--user $(shell id --user):$(shell id --group) \
		--mount type=bind,source="$(shell pwd)/backend",target=/app \
		${NAME}_bootstrap \
		bash -ceuxo pipefail " \
			gpg \
				--quick-generate-key \
				--batch \
				--pinentry-mode loopback \
				--passphrase ${GNUPG_SECRET_SIGNING_KEY_PASSPHRASE} \
				'${PERSON} (${COMMENT}) <${EMAIL}>' \
				ed25519 \
				sign \
				never && \
			fingerprint=\$$(gpg \
					--list-secret-keys \
					--with-colons \
					--keyid-format=long \
					${EMAIL} \
				| grep \
					--before=3 \
					'${PERSON} (${COMMENT}) <${EMAIL}>' \
				| awk -F: '\$$1==\"fpr\" {printf \$$10; exit}') && \
			echo fingerprint=\$${fingerprint} && \
			mkdir --parents \
				./backend/src/gpg-keys && \
			gpg \
				--batch \
				--pinentry-mode loopback \
				--passphrase ${GNUPG_SECRET_SIGNING_KEY_PASSPHRASE} \
				--armor \
				--export-secret-keys \$${fingerprint} \
			> /app/src/gpg-keys/\$${fingerprint}.gpg \
		"
