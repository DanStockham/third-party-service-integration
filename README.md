## How to deploy service

### Using Docker

In order to run the API from a docker container, you'll have to create the image first

* `docker build -f ./CUNAMUTUAL_TAKEHOME/Dockerfile -t take-home .`

Once the image is created, then you can manually start the docker container

* `docker run -d -p 5000:80 take-home`

### From the binary

In the terminal, navigate to the project folder and first do a build

* `cd path/to/project`

* `dotnet build`

And then finally run the service

* `dotnet run --project CUNAMUTUAL_TAKEHOME`
