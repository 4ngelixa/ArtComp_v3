FROM gcr.io/google-appengine/aspnetcore:2.1
COPY . /app
WORKDIR /app
ENTRYPOINT ["dotnet", "eti-asg2.dll"]