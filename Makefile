build-api:
	docker build -t ory-api:latest ./api
build-auth:
	docker build -t ory-tl-auth:latest ../AuthenticationServer/Adopto.Auth.Server