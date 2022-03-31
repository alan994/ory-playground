# ory-playground

Steps to recreate hydrator mutator problem

1. Run `make build-api`
2. Run `make build-client`
3. Run `docker-compose -f db-compose.yaml -p ory-db up -d`
4. Login to postgres db and create db named `kratos`
5. Run `docker-compose -f docker-compose.yaml -p ory-services up -d`
6. Open browser on http://127.0.0.1:4202 and clik on `Login`
7. In newly openned tab go through registration proces
8. Come back to http://127.0.0.1:4020 and click on `Call secure API endpoint`


You should see that `Oath Keeper` called `/hydrator` endpoint and after that it forward two headers to API `/secure` endpoint but without values.
