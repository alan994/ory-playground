build-api:
	docker build -t ory-api:latest ./api

build-client:
	docker build -t ory-alan-ui:latest ./alan-ui

ory-db-up:
	docker-compose -p ory-db -f db-compose.yaml up -d

ory-services-up:
	docker-compose -p ory-services -f docker-compose.yaml up -d

