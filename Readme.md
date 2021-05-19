[Tạo Docker Image]
docker build -t huu1995edu/coreapiselenium .
[Run Docker Image với Docker Container]
docker run -d -p 8080:80 huu1995edu/coreapiselenium
[List docker image]
docker ps
[Stop iamge]
docker stop [iddocker]
[Push/Pull docker]
docker [push/pull] huu1995edu/coreapiselenium
[list file]
docker exec -it [id] bash -> ls
[clear cache]
docker builder prune
