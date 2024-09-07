docker network create rabbitnet
docker run -d --network rabbitnet
docker logs some-rabbit


```
graylog:      
    volumes:      
      -    graylog_example.conf :/usr/share/graylog/data/config/graylog.conf
```

    environment:     
	- GRAYLOG_ELASTICSEARCH_VERSION=7 
      - GRAYLOG_PASSWORD_SECRET=somepasswordpepper      
      - GRAYLOG_ROOT_PASSWORD_SHA2=8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918
      - GRAYLOG_HTTP_EXTERNAL_URI=http://127.0.0.1:9000/
      - GRAYLOG_HTTP_ENABLE_CORS=true
      - GRAYLOG_METRICS_PROMETHEUS_ENABLED=true
      - GRAYLOG_ELASTICSEARCH_HOSTS=http://opensearch:9200
      - GRAYLOG_SKIP_PREFLIGHT_CHECKS=true
      - GRAYLOG_INTEGRATIONS_SCRIPTS_DIR=/usr/share/graylog/scripts

docker run -d -p 9200:9200 -p 9600:9600 -e "discovery.type=single-node" -e "OPENSEARCH_INITIAL_ADMIN_PASSWORD=thYud4534_?" opensearchproject/opensearch:2.12.0

# Graylog

https://go2docs.graylog.org/current/downloading_and_installing_graylog/docker_installation.htm?TocPath=Installing%20Graylog%7C_____2

Graylog is a powerful open source log management platform that allows easy log management of both structured and unstructured data.

You will need a recent version of Docker and the following Docker images to use Graylog.

docker pull graylog/graylog
docker pull mongo
docker pull opensearchproject/opensearch:2
docker pull opensearchproject/opensearch-dashboards:2

Warning: We caution you not to install or upgrade to OpenSearch 2.16! It is not supported. Doing so will break your instance!

Warning: All configuration examples are created to run on a local computer. Should those be used on external servers, adjust GRAYLOG_HTTP_EXTERNAL_URI and add GRAYLOG_HTTP_PUBLISH_URI and GRAYLOG_HTTP_EXTERNAL_URI according to the server.conf documentation.

# How to Get in Log Data

docker run --link mongo --link opensearch \    
   -p 9000:9000 -p 12201:12201 -p 1514:1514 -p 5555:5555 \    
   -e GRAYLOG_HTTP_EXTERNAL_URI="http://127.0.0.1:9000/" \    
   -d graylog/graylog

After ensuring that your Graylog Docker container is listening on :5555, create a raw/plaintext input by navigating to your Graylog port, e.g. https://localhost:9000/system/inputs.

Once on the Inputs page, search for Raw/Plaintext TCP and click Launch new input.

After launching the input, a configuration form will pop up with several options. You can leave most of these options as their defaults, but note that you’ll need to provide a name for the input, as well as select the node, or “Global” for the location for the input.

After that you can send a plain text message to the Graylog raw/plaintext TCP input running on port 5555 using the following command:

echo 'First log message' | nc localhost 5555

# Settings

Graylog comes with a default configuration that works out of the box, but you have to set a password for the admin user and the web interface needs to know how to connect from your browser to the Graylog REST API.

Both settings can be configured via environment variables

-e GRAYLOG_ROOT_PASSWORD_SHA2=8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918
-e GRAYLOG_HTTP_EXTERNAL_URI="http://127.0.0.1:9000/"

In this case you can log into Graylog with the username and password admin.

Generate your own admin password with the following command and put the SHA-256 hash into the GRAYLOG_ROOT_PASSWORD_SHA2 environment variable:

echo -n "Enter Password: " && head -1 < /dev/stdin | tr -d '\n' | sha256sum | cut -d " " -f1

All these settings and command line parameters can be put in a docker-compose.yml file, so that they don’t have to be executed one after the other.

```yaml
version: '3'

services:
  # MongoDB: https://hub.docker.com/_/mongo/
  mongodb:
    image: "mongo:6.0.14"
    volumes:
      - "mongodb_data:/data/db"
    restart: "on-failure"
    networks:
      - graylog

  opensearch:
    image: "opensearchproject/opensearch:2.12.0"
    environment:
      - "OPENSEARCH_JAVA_OPTS=-Xms1g -Xmx1g"
      - "bootstrap.memory_lock=true"
      - "discovery.type=single-node"
      - "action.auto_create_index=false"
      - "plugins.security.ssl.http.enabled=false"
      - "plugins.security.disabled=true"
      # Can generate a password for `OPENSEARCH_INITIAL_ADMIN_PASSWORD` using a linux device via:
      # tr -dc A-Z-a-z-0-9_@#%^-_=+ < /dev/urandom | head -c${1:-32}
      - OPENSEARCH_INITIAL_ADMIN_PASSWORD=+_8r#wliY3Pv5-HMIf4qzXImYzZf-M=M
    ulimits:
      memlock:
        hard: -1
        soft: -1
      nofile:
        soft: 65536
        hard: 65536
    restart: "on-failure"
    networks:
      - graylog

  # Graylog: https://hub.docker.com/r/graylog/graylog/
  graylog:
    hostname: "server"
    image: "graylog/graylog-enterprise:6.0"
    # To install Graylog Open: "graylog/graylog:6.0"
    depends_on:
      mongodb:
        condition: "service_started"
      opensearch:
        condition: "service_started"
    entrypoint: "/usr/bin/tini -- wait-for-it elasticsearch:9200 -- /docker-entrypoint.sh"
    environment:
      - GRAYLOG_NODE_ID_FILE="/usr/share/graylog/data/config/node-id"
      - GRAYLOG_HTTP_BIND_ADDRESS="0.0.0.0:9000"
      - GRAYLOG_ELASTICSEARCH_HOSTS="http://opensearch:9200"
      - GRAYLOG_MONGODB_URI="mongodb://mongodb:27017/graylog"
      # To make reporting (headless_shell) work inside a Docker container
      - GRAYLOG_REPORT_DISABLE_SANDBOX="true"
      # CHANGE ME (must be at least 16 characters)!
      - GRAYLOG_PASSWORD_SECRET="somepasswordpepper"
      # Password: "admin"
      - GRAYLOG_ROOT_PASSWORD_SHA2="8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918"
      - GRAYLOG_HTTP_EXTERNAL_URI="http://127.0.0.1:9000/"
    ports:
      # Graylog web interface and REST API
      - "9000:9000/tcp"
      # Beats
      - "5044:5044/tcp"
      # Syslog TCP
      - "5140:5140/tcp"
      # Syslog UDP
      - "5140:5140/udp"
      # GELF TCP
      - "12201:12201/tcp"
      # GELF UDP
      - "12201:12201/udp"
      # Forwarder data
      - "13301:13301/tcp"
      # Forwarder config
      - "13302:13302/tcp"
    volumes:
      - "graylog_data:/usr/share/graylog/data/data"
      - "graylog_journal:/usr/share/graylog/data/journal"
    restart: "on-failure"

networks:
  graylog:
    driver: "bridge"

volumes:
  mongodb_data:
  graylog_data:
  graylog_journal:
```

After starting all three Docker containers by running docker-compose up, you can open the URL https://127.0.0.1:9000 in a web browser and log in with username admin and password admin (make sure to change the password later). Change GRAYLOG_HTTP_EXTERNAL_URI= to your server IP if you run Docker remotely.

## Persisting Data

In order to make the recorded data persistent, you can use external volumes to store all data. In the case of a container restart, this will simply re-use the existing data from the former instances.

Using Docker volumes for the data of MongoDB, OpenSearch, and Graylog, the docker-compose.yml file looks as follows:

```yaml
version: '3'
services:
  # MongoDB: https://hub.docker.com/_/mongo/
  mongodb:
    image: mongo:6.0.14
    volumes:
      - mongo_data:/data/db

  # OpenSearch: https://opensearch.org/docs/latest/install-and-configure/install-opensearch/docker/
  opensearch:
    image: opensearchproject/opensearch:2.12.0
    volumes:
      - os_data:/usr/share/opensearch/data
    environment:
      - http.host=0.0.0.0
      - transport.host=localhost
      - network.host=0.0.0.0
      - "OPENSEARCH_JAVA_OPTS=-Xms512m -Xmx512m"
      - OPENSEARCH_INITIAL_ADMIN_PASSWORD=${OPENSEARCH_INITIAL_ADMIN_PASSWORD}
    ulimits:
      memlock:
        soft: -1
        hard: -1
      mem_limit: 1g

  # Graylog: https://hub.docker.com/r/graylog/graylog/
  graylog:
    image: graylog/graylog-enterprise:6.0
  #    To install Graylog Open: "graylog/graylog:6.0"
    volumes:
      - graylog_data:/usr/share/graylog/data
    environment:
      # CHANGE ME (must be at least 16 characters)!
      - GRAYLOG_PASSWORD_SECRET=somepasswordpepper
      # Password: admin
      - GRAYLOG_ROOT_PASSWORD_SHA2=8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918
      - GRAYLOG_HTTP_EXTERNAL_URI=http://127.0.0.1:9000/
    entrypoint: /usr/bin/tini -- wait-for-it opensearch:9200 --  /docker-entrypoint.sh
    links:
      - mongodb:mongo
      - opensearch
    restart: always
    depends_on:
      - mongodb
      - opensearch
    ports:
      # Graylog web interface and REST API
      - 9000:9000
      # Syslog TCP
      - 1514:1514
      # Syslog UDP
      - 1514:1514/udp
      # GELF TCP
      - 12201:12201
      # GELF UDP
      - 12201:12201/udp

  # Volumes for persisting data, see https://docs.docker.com/engine/admin/volumes/volumes/
  volumes:
    mongo_data:
      driver: local
    os_data:
      driver: local
    graylog_data:
      driver: local
```

Start all services with exposed data directories.

docker-compose up





# 2nd Document: What is Graylog:



Graylog’s preferred Log Format — GELF — is also supported by Docker natively. So if you are using Docker logs already (Docker’s internal logging functionality) you can forward all logs from your container to the specified GELF endpoint.

GRAYLOG SETUP:

Create the following docker compose file (yaml):

```yaml
version: '2'
services:
 # MongoDB: https://hub.docker.com/_/mongo/
 mongodb:
 image: mongo:4.2
 networks:
 - graylog
 #DB in share for persistence
 volumes:
 - /mongo_data:/data/db
 # Elasticsearch:
https://www.elastic.co/guide/en/elasticsearch/reference/7.10/docker.html
 elasticsearch:
 image: docker.elastic.co/elasticsearch/elasticsearch-oss:7.10.2
 #data folder in share for persistence
 volumes:
 - /es_data:/usr/share/elasticsearch/data
 environment:
 - http.host=0.0.0.0
 - transport.host=localhost
 - network.host=0.0.0.0
 - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
 ulimits:
 memlock:
 soft: -1
 hard: -1
 mem_limit: 1g
 networks:
 - graylog
 # Graylog: https://hub.docker.com/r/graylog/graylog/
 graylog:
 image: graylog/graylog:4.2
 #journal and config directories in local NFS share for persistence
 volumes:
 - /graylog_journal:/usr/share/graylog/data/journal
 environment:
 # CHANGE ME (must be at least 16 characters)!
 - GRAYLOG_PASSWORD_SECRET=somepasswordpepper
 # Password: admin
 - GRAYLOG_ROOT_PASSWORD_SHA2=
60eca706ef1bd535119e937edb0730824b6630ca444e14407e6f63be8c0e3ab2
 - GRAYLOG_HTTP_EXTERNAL_URI=http://localhost:9000/
 entrypoint: /usr/bin/tini - wait-for-it elasticsearch:9200 - /dockerentrypoint.sh
 networks:
 - graylog
 links:
 - mongodb:mongo
 - elasticsearch
 restart: always
 depends_on:
 - mongodb
 - elasticsearch
 ports:
 # Graylog web interface and REST API
 - 9000:9000
 # Syslog TCP
 - 1514:1514
 # Syslog UDP
 - 1514:1514/udp
 # GELF TCP
 - 12201:12201
 # GELF UDP
 - 12201:12201/udp
# Volumes for persisting data, see
https://docs.docker.com/engine/admin/volumes/volumes/
volumes:
 mongo_data:
 driver: local
 es_data:
 driver: local
 graylog_journal:
 driver: local
networks:
 graylog:
 driver: bridge
```

To create graylog_password_secret_sha2, use the below command:

To create graylog_password_secret_sha2, use the below command:

Create Persistent volumes and set the right permissions for each of them:

sudo mkdir /mongo_data
sudo mkdir /es_data
sudo mkdir /graylog_journal
sudo chmod 777 -R /mongo_data
sudo chmod 777 -R /es_data
sudo chmod 777 -R /graylog_journal


Run the docker-compose.yml file using the below command:

docker-compose up -d

Once all the images have been pulled and containers started, check the status with the below command:

docker ps

Run the image using the command:

docker run -d -p 8051:5000 --log-driver gelf --log-opt gelfaddress=udp://IP_address:12201 imagename

Access the Graylog server using : http://IP_address:9000

Enter the username: admin

password: used in graylog_password_Sha2 as set in yaml file.

Once logged in, go to system/Inputs to see the logs of the running image. http://IP_address:9000/system/inputs