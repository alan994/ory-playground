build-api:
	docker build -t ory-api:latest ./api

ory-db-up:
	docker-compose -p ory-db -f db-compose.yaml up -d

ory-services-up:
	docker-compose -p ory-services -f docker-compose.yaml up -d

