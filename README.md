# Database

This repository presents the reference implementation of a product data server
as part of the product data network
[buildingenvelopedata.org](https://www.buildingenvelopedata.org/). Before
deploying this repository,
[machine](https://github.com/building-envelope-data/machine) can be used to set
up the machine.

The [API specification of the product data
servers](https://github.com/building-envelope-data/api/blob/develop/apis/database.graphql)
is available in the repository
[api](https://github.com/building-envelope-data/api). There is also
a [visualization of the API of a product data
server](https://graphql-kit.com/graphql-voyager/?url=https://www.solarbuildingenvelopes.com/graphql/).

This repository is deployed as the [product data server of TestLab Solar
Facades of Fraunhofer ISE](https://www.solarbuildingenvelopes.com).

If you have a question for which you don't find the answer in this repository,
please raise a [new
issue](https://github.com/building-envelope-data/database/issues/new) and add
the tag `question`! All ways to contribute are presented by
[CONTRIBUTING.md](https://github.com/building-envelope-data/database/blob/develop/CONTRIBUTING.md).
The basis for our collaboration is decribed by our [Code of
Conduct](https://github.com/building-envelope-data/database/blob/develop/CODE_OF_CONDUCT.md).

## Contents

[Getting started](#getting-started)

- [On your Linux machine](#on-your-linux-machine)
- [Migrating the Database](#migrating-the-database)
- [Developing with Visual Studio Code](#developing-with-visual-studio-code)
- [Troubleshooting](#troubleshooting)

[Deployment](#deployment)

- [Setting up a Debian production machine](#setting-up-a-debian-production-machine)
- [Creating a release](#creating-a-release)
- [Deploying a release](#deploying-a-release)
- [Troubleshooting](#troubleshooting-1)

## Getting started

### On your Linux machine

1. Open your favorite shell, for example, good old
   [Bourne Again SHell, aka, `bash`](https://www.gnu.org/software/bash/),
   the somewhat newer
   [Z shell, aka, `zsh`](https://www.zsh.org/),
   or shiny new
   [`fish`](https://fishshell.com/).

1. Install [Git](https://git-scm.com/) by running
   `sudo apt install git-all` on [Debian](https://www.debian.org/)-based
   distributions like [Ubuntu](https://ubuntu.com/), or
   `sudo dnf install git` on [Fedora](https://getfedora.org/) and closely-related
   [RPM-Package-Manager](https://rpm.org/)-based distributions like
   [CentOS](https://www.centos.org/). For further information see
   [Installing Git](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

1. Clone the source code by running
   `git clone git@github.com:building-envelope-data/database.git` and navigate
   into the new directory `database` by running `cd ./database`.

1. Initialize, fetch, and checkout possibly-nested submodules by running
   `git submodule update --init --recursive`. An alternative would have been
   passing `--recurse-submodules` to `git clone` above.

1. Prepare your environment by running `cp ./.env.development.sample ./.env && chmod 600 ./.env`,
   `cp ./frontend/.env.local.development.sample ./frontend/.env.local && chmod 600 ./frontend/.env.local`,
   and adding the line `127.0.0.1 local.solarbuildingenvelopes.com` to your
   `/etc/hosts` file.

1. Prepare your remote controls GNU Make and Docker Compose by running

   - `ln --symbolic ./docker.mk ./Makefile` and
   - `ln --symbolic ./docker-compose.development.yaml ./docker-compose.yaml`.

1. Install [Docker Desktop](https://www.docker.com/products/docker-desktop), and
   [GNU Make](https://www.gnu.org/software/make/).

1. List all GNU Make targets by running `make help`.

1. Generate and trust a self-signed certificate authority and SSL certificates
   by running `./certificates.mk ssl`. If you are locally working on the metabase and the
   database and if you need them to communicate over HTTPS, then instead of
   running `./certificates.mk ssl`, make the `CERTIFICATE_AUTHORITY_*` variable
   values in the `.env` file match the ones from the metabase (these variables
   match in the `.env.development.sample` files), copy the certificate
   authority files from the directories `./ssl`, `./backend/ssl`, and
   `./frontend/ssl` of the metabase project into the respective directories in
   the database project (if the repository reside alongside each other by
   running `mkdir ./ssl ./backend/ssl ./frontend/ssl && cp ../metabase/ssl/ca.* ./ssl && cp ../metabase/backend/ssl/ca.* ./backend/ssl && cp ../metabase/frontend/ssl/ca.* ./frontend/ssl`), and run the command
   `./certificates.mk generate-ssl-certificate`.

1. Generate JSON Web Token (JWT) encryption and signing certificates by running
   `./certificates.mk jwt`.

1. Generate and export a GnuPG key with the passphrase
   `${GNUPG_SECRET_SIGNING_KEY_PASSPHRASE}` set in the `./.env` file to the
   file `./backend/src/gpg-keys/<KEY_FINGERPRINT>.gpg` by running
   `./gpg.mk key PERSON=${name} COMMENT=${comment} EMAIL=${email}` with your information
   filled in, for example,
   `./gpg.mk key NAME="Anna Smith" COMMENT=first EMAIL=anna.smith@fraunhofer.de`. Then copy the key's fingerprint which
   is output by the command and set it as the value of the
   `GNUPG_SECRET_SIGNING_KEY_FINGERPRINT` variable in the `./.env` file.

   Instead of using the GNU Make target `key`, you may

   1. install [GnuPG](https://gnupg.org) as described on
      [Download GnuPG](https://gnupg.org/download/index.html) or
      [GnuPG Package Repositories](https://www.gnupg.org/blog/20250827-new-repository.html),
   1. generate a GnuPG key by running `gpg --full-generate-key` and answering
      the prompts,
   1. list the keys for which you have both a public and secret key by running
      `gpg --list-secret-keys --keyid-format=long`,
   1. identify and copy the long form of the fingerprint of the key you have
      just created,
   1. remember the key in the variable `fingerprint` by running
      `fingerprint=<KEY_FINGERPRINT>` with `<KEY_FINGERPRINT>` replaced by the
      copied fingerprint,
   1. create the directory to save the secret key to if it does not exist yet
      by running `mkdir --parents ./backend/src/gpg-keys`,
   1. export the armored secret key to the file
      `./backend/src/gpg-keys/${fingerprint}.gpg` by running
      `gpg --armor --export-secret-keys ${fingerprint} > ./backend/src/gpg-keys/${fingerprint}.gpg`,
   1. Set the value of the variable `GNUPG_SECRET_SIGNING_KEY_FINGERPRINT` in
      the `./.env` file to the remembered fingerprint in your favorite editor.

1. Create the PostgreSQL database and schema by running
   `./database.mk createdb migrate`.

1. Build and start all services and follow their logs by running
   `make build up logs`.

1. In your web browser, navigate to the

   - web frontend at `https://local.solarbuildingenvelopes.com:5051`,
   - GraphQL API at `https://local.solarbuildingenvelopes.com:5051/graphql/`,
   - REST API `https://local.solarbuildingenvelopes.com:5051/openapi/docs//`,
   - dummy email server at `https://local.solarbuildingenvelopes.com:5051/email/`
     (to view for example the confirmation email sent during registration),
   - OpenId Connect configuration navigate to
     `https://local.solarbuildingenvelopes.com:5051/.well-known/openid-configuration`

   Note that the port is `5051` by default. If you set the variable
   `HTTPS_PORT` within the `./.env` to some other value though, you need to use
   that value instead within the URLs.

In another shell

1. Drop into `bash` with the working directory `/app`, which is mounted to the
   host's `./backend` directory, inside a fresh Docker container based on
   `./backend/Dockerfile` by running `make shell SERVICE=backend`. If
   necessary, the Docker image is (re)built automatically, which takes a while
   the first time. Note that the Docker image and containers try to use the
   same user and group IDs as the ones on the host machine. This has the upside
   that files created within containers in mounted directories are owned by the
   host user. It has the downside that the Docker image may fail to build
   because the IDs may already be taken by other users and groups in the base
   image. This happens for example if you are `root` on the host machine with
   the user and group IDs 0. If there is an ID collision, then you can either
   change the user and group ID on the host machine (for example by logging in
   as another user) or you can replace all occurrences of `shell id --group`
   and `shell id --user` in `Makefile` by fixed non-colliding IDs like 1000. If
   you know a better way, please
   [let use know on GitHub](https://github.com/building-envelope-data/database/issues/new).
1. List all backend GNU Make targets by running `make help`.
1. For example, update packages and tools by running `make update`.
1. Drop out of the container by running `exit` or pressing `Ctrl-D`.

### Migrating the Database

After changing the domain model in `./backend/src/data`, you need to migrate
the database by dropping into `make shell SERVICE=backend`, adding a migration
with `make migration NAME=${MIGRATION_NAME}`, verifying and if necessary
adapting the new migration C# code and SQL scripts, exiting the container with
`exit`, and applying the new migration to the PostgreSQL database with
`./database.mk migrate`. See
[Migrations Overview](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
and the following pages for details.

### Developing with Visual Studio Code

On the very first usage:

1. Install [Visual Studio Code](https://code.visualstudio.com) and open it.
   Navigate to the Extensions pane (`Ctrl+Shift+X`). Add the extension
   [Remote Development](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.vscode-remote-extensionpack).
1. Navigate to the
   [Remote Explorer](https://code.visualstudio.com/docs/devcontainers/containers#_managing-containers)
   pane. Hover over the running `database-backend-*` container (if it is not
   running, then run `make up` in a shell inside the project directory) and
   click on the "Attach in Current Window" icon. In the Explorer pane, open the
   directory `/app`, which is mounted to the host's `./backend` directory.
   Navigate to the Extensions pane. Add the extensions
   [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit),
   [IntelliCode for C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscodeintellicode-csharp),
   [GraphQL: Language Feature Support](https://marketplace.visualstudio.com/items?itemName=GraphQL.vscode-graphql),
   and
   [GitLens — Git supercharged](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens).
1. Navigate to the
   [Remote Explorer](https://code.visualstudio.com/docs/devcontainers/containers#_managing-containers)
   pane. Hover over the running `database-frontend-*` container and click on
   the "Attach in New Window" icon. In the Explorer pane, open the directory
   `/app`, which is mounted to the host's `./frontend` directory. Navigate to
   the Extensions pane. Add the extensions
   [GraphQL: Language Feature Support](https://marketplace.visualstudio.com/items?itemName=GraphQL.vscode-graphql),
   and
   [GitLens — Git supercharged](https://marketplace.visualstudio.com/items?itemName=eamodio.gitlens).

Note that the Docker containers are configured in `./docker-compose.development.yaml` in
such a way that Visual Studio Code extensions installed within containers are
retained in Docker volumes and thus remain installed across `make down` and
`make up` cycles.

On subsequent usages: Open Visual Studio Code, navigate to the "Remote
Explorer" pane, and attach to the container(s) you want to work in.

The following Visual Studio Code docs may be of interest for productivity and
debugging

- [Developing inside a Container](https://code.visualstudio.com/docs/devcontainers/containers)
- [Git](https://code.visualstudio.com/docs/sourcecontrol/overview)
- [C#](https://code.visualstudio.com/docs/csharp/navigate-edit)
- [TypeScript](https://code.visualstudio.com/docs/typescript/typescript-tutorial)

#### Debugging

To debug the
[ASP.NET Core web application](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core),
attach Visual Studio Code to the `database-backend-*` container,
[press `Ctrl+Shift+P`, select "Debug: Attach to a .NET 5+ or .NET Core process"](https://code.visualstudio.com/docs/csharp/debugging#_attaching-to-a-process),
and choose the process `/app/src/bin/Debug/net10.0/Database run` titled
`Database` or alternatively navigate to the "Run and Debug" pane
(`Ctrl+Shift+D`), select the launch profile ".NET Core Attach", press the
"Start Debugging" icon (`F5`), and select the same process as above. Then, for
example, open some source files to set breakpoints, navigate through the
website https://local.solarbuildingenvelopes.com:5051, which will stop at
breakpoints, and inspect the information provided by the debugger at the
breakpoints. For details on debugging C# in Visual Studio Code, see
[Debugging](https://code.visualstudio.com/docs/csharp/debugging).

Note that the debugger detaches after the
[polling file watcher](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-watch#environment-variables)
restarts the process, which happens for example after editing a source file
because `dotnet watch` is configured in `./docker-compose.development.yaml` with
`DOTNET_USE_POLLING_FILE_WATCHER` set to `true`. As of this writing, there is
an
[open feature request to reattach the debugger automatically](https://github.com/dotnet/vscode-csharp/issues/4822).
There also are multiple extensions like
[.NET Watch Attach](https://marketplace.visualstudio.com/items?itemName=Trottero.dotnetwatchattach)
and
[.NET Stalker Debugger](https://marketplace.visualstudio.com/items?itemName=spencerjames.stalker-debugger)
that attempt to solve that. Those extensions don't work in our case though, as
they try to restart `dotnet watch` themselves, instead of waiting for the
polling file watcher of `dotnet watch` to restart
`/app/src/bin/Debug/net10.0/Database run` and attach to that process.

### Troubleshooting

After migrating the PostgreSQL database or changing the `database` schema
manually or upgrading Npgsql, the service `backend` may throw exceptions
regarding the object-relational mapping (Npgsql or EF Core). In that case it
may be necessary to restart the service `backend`, for example, by running
`make down up` and it may even be necessary recreate the database from scratch
by running `make down remove-data-volume up`. Note that the latter will remove
all data from PostgreSQL, recreate the database and its schema, and seed it
freshly.

When your hard-disk starts to grow full, it may be the case that Docker does
not clean-up anonymous volumes properly. You can do so manually by running
`docker system prune` potentially with the arguments `--volumes` and/or
`--all`. Note that this may result in loss of data. It may also be the case
that the log files grew huge. You can delete them by running
`rm ./backend/src/logs/*`.

When the `frontend` Docker image does not build in production because of an
unused import in an automatically generated file, for example, one in the
directory `./frontend/__generated__`, then **temporarily** ignore TypeScript
build errors by adding the following lines to `./frontend/next.config.ts`, for
example with `vi` or `nano` in a shell on the deployment machine:

```
typescript: {
  ignoreBuildErrors: true,
},
```

The same can happen in development when running `make build` (or `yarn run build`) in the shell entered by `make shell SERVICE=frontend`. In that case,
remove the offending import manually in the file and try again, for example
using tail like so `tail -n +5 ./__generated__/queries/... > x.tmp && mv x.tmp ...` . Do not disable build errors in development because when you do so, build
errors in non-generated files may leak into the code base.

## Deployment

For information on using Docker in production see
[Configure and troubleshoot the Docker daemon](https://docs.docker.com/config/daemon/)
and the pages following it.

### Setting up a Debian production machine

1. Use the sibling project [machine](https://github.com/building-envelope-data/machine) and its
   instructions for the first stage of the set-up.
1. Enter a shell on the production machine using `ssh`.
1. Change into the directory `/app` by running `cd /app`.
1. Clone the repository twice by running
   ```
   for environment in staging production ; do
     git clone git@github.com:building-envelope-data/database.git ./${environment}
   done
   ```
1. For each of the two environments staging and production referred to by
   `${environment}` below:
   1. Set the variable `environment` by running `environment=staging` or
      `environment=production`.

   1. Change into the clone `${environment}` by running `cd /app/${environment}`.

   1. Open `https://www.buildingenvelopedata.org` in your favorite web browser,
      log into your account, navigate to the institution operating this
      database (which you should be a representative of), add an OpenId Connect
      Application with

      - client ID and display name of your choice;
      - consent type: explicit;
      - endpoints: authorization, pushed authorization, introspection,
        end session, revocation, token;
      - grant types: authorization code and refresh token;
      - response types: code;
      - scopes: profile, read:api, write:api, api:database:manage;
      - requirements: proof key for code exchange and pushed authorization
        requests;
      - post logout redirect URI: `https://${HOST}/connect/callback/logout/metabase`
      - redirect URI: `https://${HOST}/connect/callback/login/metabase`
        where `${HOST}` is the domain name with sub-domain of the deployment,
        for example, `staging.solarbuildingenvelopes.com` or
        `www.solarbuildingenvelopes.com` for the product-data database of the
        TestLab Solar Facades.

      Alternatively, after logging in, open
      `https://www.buildingenvelopedata.org/graphql/` and run the following
      mutation with your institution ID and host filled-in:

      ```
      mutation {
        createOpenIdConnectApplication(
          input: {
            institutionId: "00000000-0000-0000-0000-000000000000"
            clientId: "my-client"
            consentType: EXPLICIT
            displayName: "My Client"
            endpoints: [AUTHORIZATION, PUSHED_AUTHORIZATION, INTROSPECTION, END_SESSION, REVOCATION, TOKEN]
            grantTypes: [AUTHORIZATION_CODE, REFRESH_TOKEN]
            postLogoutRedirectUri: "https://${HOST}/connect/callback/logout/metabase"
            redirectUri: "https://${HOST}/connect/callback/login/metabase"
            responseTypes: [CODE]
            scopes: [PROFILE, READ_API]
          }
        ) {
          clientSecret
          errors {
            code
            message
            path
          }
        }
      }
      ```

   1. Prepare the environment by running
      `cp ./.env.${environment}.sample ./.env && chmod 600 ./.env`,
      `cp ./frontend/.env.local.${environment}.sample ./frontend/.env.local && chmod 600 ./frontend/.env.local`,
      and by adjusting variable values in the copies to your needs, in
      particular, by setting passwords to newly generated ones, where random
      passwords may be generated by running `openssl rand -base64 32`. Here is
      some information on what the variables mean

      - `NAME` is the name Docker project name, in particular, it is the prefix
        of the Docker container names listed by `docker ps --all`;
      - `HOST` is the domain name with sub-domain of the deployment, in
        particular, it is used to make resource locators absolute;
      - `HTTP_PORT` is the HTTP port to which the reverse proxy NGINX forwards
        for HTTPS requests (see `PRODUCTION_HTTP_PORT` and
        `STAGING_HTTP_PORT` in the `.env` file of your clone of
        [machine](https://github.com/building-envelope-data/machine));
      - `METABASE_HOST` is the domain name with sub-domain of the metabase, in
        particular, to use it as OpenId Connect provider and to ask it for
        information about logged-in users needed for authorization;
      - `DATABASE_ID` is the UUID that was assigned to this product-data
        database upon registering it at the metabase;
      - `OPERATOR_ID` is the UUID of the institution that operates this
        product-data database;
      - `VERIFICATION_CODE` is the verification code that was generated for
        this product-data database upon registering it at the metabase;
      - `OPEN_ID_CONNECT_CLIENT_ID` and `OPEN_ID_CONNECT_CLIENT_SECRET` are the
        OpenId Connect client identifier and secret of this product-data
        database as a client of the metabase acting as identity provider (the
        client secret is given when registering an OpenId Connect client at
        the metabase);
      - `GNUPG_SECRET_SIGNING_KEY_FINGERPRINT` and
        `GNUPG_SECRET_SIGNING_KEY_PASSPHRASE` are fingerprint and passphrase
        of the exported GnuPG secret key for signing in the file
        `./backend/src/gpg-keys/${GNUPG_SECRET_SIGNING_KEY_FINGERPRINT}.gpg`;
      - `JSON_WEB_TOKEN_ENCRYPTION_CERTIFICATE_PASSWORD` and
        `JSON_WEB_TOKEN_SIGNING_CERTIFICATE_PASSWORD` are passwords used to
        encrypt and sign JSON web tokens (JWT) used by OpenId Connect;
      - `SMTP_HOST` and `SMTP_PORT` are host and port of the message transfer
        agent to be used to send emails through the Simple Mail Transfer
        Protocol (SMTP);
      - `RELAY_SMTP_HOST`, `RELAY_SMTP_PORT`, and `RELAY_ALLOWED_EMAILS` are
        host and port of the message transfer agent and a list of allowed
        email addresses to send emails to even in the staging environment.

   1. Prepare your remote controls GNU Make and Docker Compose by running

      - `ln --symbolic ./docker.mk ./Makefile` and
      - `ln --symbolic ./docker-compose.production.yaml ./docker-compose.yaml`.

   1. Generate JSON Web Token (JWT) encryption and signing certificates by running
      `./certificates.mk jwt`.

   1. Generate and export a GnuPG key with the passphrase
      `${GNUPG_SECRET_SIGNING_KEY_PASSPHRASE}` set in the `./.env` file to the
      file `./backend/src/gpg-keys/<KEY_FINGERPRINT>.gpg` by running `./gpg.mk PERSON=${name} COMMENT=${comment} EMAIL=${email} key` with your information
      filled in, for example, `make gpg NAME="Anna Smith" COMMENT=first EMAIL=anna.smith@fraunhofer.de`. Then copy the key's fingerprint which
      is output by the command and set it as the value of the
      `GNUPG_SECRET_SIGNING_KEY_FINGERPRINT` variable in the `./.env` file.

      Instead of using the GNU Make target `gpg`, you may

      1. install [GnuPG](https://gnupg.org) as described on
         [Download GnuPG](https://gnupg.org/download/index.html) or
         [GnuPG Package Repositories](https://www.gnupg.org/blog/20250827-new-repository.html),
      1. generate a GnuPG key by running `gpg --full-generate-key` and answering
         the prompts,
      1. list the keys for which you have both a public and secret key by running
         `gpg --list-secret-keys --keyid-format=long`,
      1. identify and copy the long form of the fingerprint of the key you have
         just created,
      1. remember the key in the variable `fingerprint` by running
         `fingerprint=<KEY_FINGERPRINT>` with `<KEY_FINGERPRINT>` replaced by the
         copied fingerprint,
      1. create the directory to save the secret key to if it does not exist yet
         by running `mkdir --parents ./backend/src/gpg-keys`,
      1. export the armored secret key to the file
         `./backend/src/gpg-keys/${fingerprint}.gpg` by running
         `gpg --armor --export-secret-keys ${fingerprint} > ./backend/src/gpg-keys/${fingerprint}.gpg`,
      1. Set the value of the variable `GNUPG_SECRET_SIGNING_KEY_FINGERPRINT` in
         the `./.env` file to the remembered fingerprint in your favorite editor.

   1. Create the PostgreSQL database by running `./database.mk createdb`.

### Creating a release

1. Draft a new release with a new version according to
   [Semantic Versioning](https://semver.org) by running the GitHub action
   [Draft a new release](https://github.com/building-envelope-data/database/actions/workflows/draft-new-release.yaml)
   which, creates a new branch named `release/v*.*.*`,
   creates a corresponding pull request, updates the
   [Changelog](https://github.com/building-envelope-data/database/blob/develop/CHANGELOG.md),
   and bumps the version in
   [`package.json`](https://github.com/building-envelope-data/database/blob/develop/frontend/package.json),
   where `*.*.*` is the version. Note that this is **not** the same as "Draft
   a new release" on
   [Releases](https://github.com/building-envelope-data/database/releases).
1. Fetch the release branch by running `git fetch` and check it out by running
   `git checkout release/v*.*.*`, where `*.*.*` is the version.
1. Apply pending migrations with `./database.mk migrate`.
1. Make sure that all tests succeed and try out any new features manually.
1. [Publish the new release](https://github.com/building-envelope-data/database/actions/workflows/publish-new-release.yaml)
   by merging the release branch into `main` whereby a new pull request from
   `main` into `develop` is created that you need to merge to finish of.

### Deploying a release

1. Enter a shell on the production machine using `ssh`.
1. Navigate into `/app/production` by running `cd /app/production`.
1. Back up the production database by running
   `./database.mk backup DIR=/app/production/backup`.
1. Change to the staging environment by running `cd /app/staging`.
1. Restore the staging database from the production backup by running
   `./database.mk restore DIR=/app/production/backup`.
1. Adapt the environment file `./.env` if necessary by comparing it with the
   `./.env.staging.sample` file of the release to be deployed.
1. Deploy the new release in the staging environment by running
   `./deploy.mk TARGET=${TAG} deploy`, where `${TAG}` is
   the release tag to be deployed, for example, `v1.0.0`.
1. If it fails _after_ the database backup was made, rollback to the previous
   state by running
   `./deploy.mk rollback`,
   figure out what went wrong, apply the necessary fixes to the codebase,
   create a new release, and try to deploy that release instead.
1. If it succeeds, deploy the new reverse proxy that handles sub-domains by
   running `cd /app/machine && make deploy` and test whether everything works
   as expected and if that is the case, continue. Note that in the
   staging environment sent emails can be viewed in the web browser under
   `https://staging.solarbuildingenvelopes.com/email/` and emails to addresses in
   the variable `RELAY_ALLOWED_EMAILS` in the `.env` file are delivered to the
   respective inboxes (the variable's value is a comma separated list of email
   addresses).
1. Change to the production environment by running `cd /app/production`.
1. Adapt the environment file `./.env` if necessary by comparing it with the
   `./.env.production.sample` file of the release to be deployed.
1. Deploy the new release in the production environment by running
   `./deploy.mk TARGET=${TAG} deploy`, where `${TAG}` is
   the release tag to be deployed, for example, `v1.0.0`.
1. If it fails _after_ the database backup was made, rollback to the previous
   state by running
   `./deploy.mk rollback`,
   figure out what went wrong, apply the necessary fixes to the codebase,
   create a new release, and try to deploy that release instead.

### Troubleshooting

The file `docker.mk` contain GNU Make targets to manage Docker containers
like `up` and `down`, to follow Docker container logs with `logs`, to drop into
shells inside running Docker containers like `shell SERVICE=backend` for the backend service
and `shell SERVICE=frontend` for the frontend service, and to list information about Docker
like `list` and `list-services`.

The Makefile `./deploy.mk` contains GNU Make targets to deploy a new release or
rollback it back as mentioned above. These targets depend on several smaller
targets like `begin-maintenance` and `end-maintenance` to begin or end
displaying maintenance information to end users that try to interact with the
website, and `backup` to backup all data before deploying a new version,
`migrate` to migrate the database, and `run-tests` to run tests.

If for some reason the website displays the maintenance page without
maintenance happening at the moment, then drop into a shell on the production
machine, check all logs for information on what happened, fix issues if
necessary, and end maintenance. It could for example happen that a cron job
set-up by [machine](https://github.com/building-envelope-data/machine) begins
maintenance, fails to do its actual job, and does not end maintenance
afterwards. Whether failing to do its job is a problem for the inner workings
of the website needs to be decided by some developer. If it for example backing
up the database fails because the machine is out of memory at the time of doing
the backup, the website itself should still working.

If the database container restarts indefinitely and its logs say

```
PANIC:  could not locate a valid checkpoint record
```

for example preceded by `LOG: invalid resource manager ID in primary checkpoint record` or `LOG: invalid primary checkpoint record`, then the database is
corrupt. For example, the write-ahead log (WAL) may be corrupt because the
database was not shut down cleanly. One solution is to restore the database
from a backup by running

```
./database.mk restore DIR=/app/data/backups/20XX-XX-XX_XX_XX_XX/
```

where the `X`s need to be replaced by proper values. Another solution is to
reset the transaction log by entering the database container with

```
docker compose run database bash
```

and dry-running

```
gosu postgres pg_resetwal --dry-run /var/lib/postgresql/data
```

and, depending on the output, also running

```
gosu postgres pg_resetwal /var/lib/postgresql/data
```

Note that both solutions may cause data to be lost.

#### Update a SQL field manually

If one field in the SQL database needs to be updated and there is no GraphQL
mutation available, then you may update it in PostgreSQL directly as
illustrated in the following example. Test it in the `staging` environment
under /app/staging before doing it in `production` under /app/production.

1. Drop into a shell on the server as user `cloud` by running
   `ssh -CvX -A cloud@IpAdressOfCloudServer`.
1. Navigate to the production environment by running `cd /app/production`.
1. Make a database backup by running `DATE=$(date +"%Y-%m-%d_%H_%M_%S")` and
   `./database.mk backup DIR=/app/data/backups/${DATE}`
1. Navigate to the staging environment by running `cd /app/staging`.
1. Load the backup into the staging database by running
   `./database.mk restore DIR=/app/data/backups/${DATE}`.
1. Drop into `psql` by running `./database.mk psql`.
1. List all tables in the schema `database` by running `\dt database.*`.
1. List all optical data records by running `select * from database.optical_data;` and remember for example one identifier of a record
   that you want to update.
1. Update a single field by running `update database.optical_data set "Description" = '...' where "Id" = 'f07499ab-f119-471f-8aad-d3c016676bce';`.
1. Delete a faulty record by running `delete from database.optical_data where "Id" = 'f07499ab-f119-471f-8aad-d3c016676bce';`.
