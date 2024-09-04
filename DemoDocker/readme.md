# Docker Compose

The Docker CLI lets you interact with your Docker Compose applications through the docker compose command, and its subcommands.

To start all the services defined in your compose.yaml file:

```zsh
% docker compose up -d
# -d: detached
```

To stop and remove the running services:

```zsh
docker compose down 
```

If you want to monitor the output of your running containers and debug issues, you can view the logs with:

```zsh
docker compose logs
```

To lists all the services along with their current status:

```zsh
docker compose ps
```

```zsh
# docker dommands
% docker --version
% docker context ls
% docker context use default
```

## Use -f to specify the name and path of one or more Compose files

```zsh
docker compose -f docker-compose.yml logs
```

## Use -p to specify a project name

```zsh
docker compose -f docker-compose.yml -p myproject up -d
```

Each configuration has a project name. Compose sets the project name using the following mechanisms, in order of precedence:

* The -p command line flag
* The COMPOSE_PROJECT_NAME environment variable
* The top level name: variable from the config file (or the last name: from a series of config files specified using -f)
* The basename of the project directory containing the config file (or containing the first config file specified using -f)
* The basename of the current directory if no config file is specified Project names must contain only lowercase letters, decimal digits, dashes, and underscores, and must begin with a lowercase letter or decimal digit. If the basename of the project directory or current directory violates this constraint, you must use one of the other mechanisms.

## Docker Compose Commands

| Command	                | Description|
|---------------------------|------------|
| docker compose alpha		| Experimental commands|
| docker compose build		| Build or rebuild services|
| docker compose config		| Parse, resolve and render compose file in canonical format|
| docker compose cp	    	| Copy files/folders between a service container and the local filesystem|
| docker compose create		| Creates containers for a service|
| docker compose down	    | Stop and remove containers, networks|
| docker compose events		| Receive real time events from containers|
| docker compose exec	    | Execute a command in a running container|
| docker compose images		| List images used by the created containers|
| docker compose kill	    | Force stop service containers|
| docker compose logs	    | View output from containers|
| docker compose ls	    	| List running compose projects|
| docker compose pause		| Pause services|
| docker compose port	    | Print the public port for a port binding|
| docker compose ps	    	| List containers|
| docker compose pull	    | Pull service images|
| docker compose push	    | Push service images|
| docker compose restart	| Restart service containers|
| docker compose rm	    	| Removes stopped service containers|
| docker compose run	    | Run a one-off command on a service|
| docker compose start		| Start services|
| docker compose stop	    | Stop services|
| docker compose top	    | Display the running processes|
| docker compose unpause	| Unpause services|
| docker compose up	    	| Create and start containers|
| docker compose version	| Show the Docker Compose version information|
| docker compose wait	    | Block until the first service container stops|
| docker compose watch		| Watch build context for service and rebuild/refresh containers when files are updated |

```zsh
# samples

docker compose -f *.yml config
docker compose -f *.yml logs
docker compose -f *.yml up
```

# Docker Compose File

The default path for a Compose file is `compose.yaml` (preferred) or `compose.yml` that is placed in the working directory. Compose also supports `docker-compose.yaml` and `docker-compose.yml` for backwards compatibility of earlier versions. If both files exist, Compose prefers the canonical `compose.yaml`.

## Version and name top-level elements

The top-level version property is defined by the Compose Specification for backward compatibility. It is only informative and you'll receive a warning message that it is obsolete if used.

```yml
version: 2
```

## Name top-level element

The top-level name property is the project name to be used if you don't set one explicitly. Compose offers a way for you to override this name (docker compose -p), and sets a default project name to be used if the top-level name element is not set.

Whenever a project name is defined by top-level name or by some custom mechanism, it is exposed for interpolation and environment variable resolution as COMPOSE_PROJECT_NAME.

```yml
name: myapp
```

## Services top-level elements

A service is an abstract definition of a computing resource within an application which can be scaled or replaced independently from other components.

```yml
name: myapp

services:
  service_name:
    image:
    volumes:
    ports:    
```

## Networks top-level elements

Networks let services communicate with each other. By default Compose sets up a single network for your app. Each container for a service joins the default network and is both reachable by other containers on that network, and discoverable by the service's name. The top-level networks element lets you configure named networks that can be reused across multiple services.

To use a network across multiple services, you must explicitly grant each service access by using the networks attribute within the services top-level element. The networks top-level element has additional syntax that provides more granular control.

```yml
name: myapp

services:
  service1:
    image: example/webapp
    networks:
      - frontend
      - backend

networks:
  frontend:
    driver: bridge
  backend:
    driver: bridge
```

### Network Drivers

* bridge: The default network driver. If you don't specify a driver, this is the type of network you are creating. Bridge networks are commonly used when your application runs in a container that needs to communicate with other containers on the same host.

* host: Remove network isolation between the container and the Docker host, and use the host's networking directly.

* overlay: Overlay networks connect multiple Docker daemons together and enable Swarm services and containers to communicate across nodes.

* ipvlan: IPvlan networks give users total control over both IPv4 and IPv6 addressing.

* macvlan: Macvlan networks allow you to assign a MAC address to a container, making it appear as a physical device on your network.

* none: Completely isolate a container from the host and other containers. none is not available for Swarm services.

### Published ports

By default, when you create or run a container using docker create or docker run, containers on bridge networks don't expose any ports to the outside world. Use the --publish or -p flag to make a port available to services outside the bridge network. This creates a firewall rule in the host, mapping a container port to a port on the Docker host to the outside world.

* `-p 8080:80`	Map port 8080 on the Docker host to TCP port 80 in the container.

* `-p 192.168.1.100:8080:80` Map port 8080 on the Docker host IP 192.168.1.100 to TCP port 80 in the container.

* `-p 8080:80/udp` Map port 8080 on the Docker host to UDP port 80 in the container.

* `-p 8080:80/tcp -p 8080:80/udp` Map TCP port 8080 on the Docker host to TCP port 80 in the container, and map UDP port 8080 on the Docker host to UDP port 80 in the container.

Publishing container ports is insecure by default. Meaning, when you publish a container's ports it becomes available not only to the Docker host, but to the outside world as well.

If you include the localhost IP address (127.0.0.1, or ::1) with the publish flag, only the Docker host and its containers can access the published container port.

## Volumes top-level element

Volumes are persistent data stores implemented by the container engine. Compose offers a neutral way for services to mount volumes, and configuration parameters to allocate them to infrastructure. The top-level volumes declaration lets you configure named volumes that can be reused across multiple services.

To use a volume across multiple services, you must explicitly grant each service access by using the volumes attribute within the services top-level element.

```yml
name: myapp

services:
  backend:
    image: example/database
    volumes:
      - db-data:/etc/data

  backup:
    image: backup-service
    volumes:
      - db-data:/var/lib/backup/data

volumes:
  db-data:
```

The db-data volume is mounted at the /var/lib/backup/data and /etc/data container paths for backup and backend respectively.

Running docker compose up creates the volume if it doesn't already exist. Otherwise, the existing volume is used and is recreated if it's manually deleted outside of Compose.

# References

* https://docs.docker.com/reference/compose-file/
* https://docs.docker.com/compose/compose-application-model/
* https://github.com/docker/awesome-compose
* https://docs.docker.com/reference/