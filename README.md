# Code Scaffolding of Entity Framework context from db
in Terminal
```
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```
To generate Models and DbContext
```
dotnet ef dbcontext scaffold "Server=DESKTOP-G14R8VA\SQLEXPRESS;Database=AnfasAPI;User Id=;Password=;Integrated Security=true;MultipleActiveResultSets=true" Microsoft.EntityFrameworkCore.SqlServer --output-dir .\Models1 --namespace AnfasAPI.Models1 --context-namespace AnfasAPI.Models1 --context-dir .\Models1 --context ApplicationDbContext --force --no-build --no-pluralize --no-onconfiguring
```

# Chrome debugging
Command to use debugging in google crome

 `--remote-debugging-port=9222`

# VS Code Extensions that are used
In case VSCode has not given recommendations yet
# Scrcpy - Display and control your  Mobile phone from PC
https://github.com/Genymobile/scrcpy


# To deploy dotnet api on IIS server
Use This Command
  - `dotnet publish --no-self-contained -r win-x64 -c Release`

Copy to IIS the files in Output path: `\bin\Release\net5.0\win-x64\publish`

  # To deploy angular in production mode
Use this Command
  - `npm install`
  - `ng build --prod --base-href /mobile/`

Copy to IIS the files in Output path: `\dist\client`
