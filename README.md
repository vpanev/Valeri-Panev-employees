# Employee Project Pair Finder
- A web application that analyzes CSV data to find pairs of employees who worked together on the same project for the longest period of time.

## Overview

This application processes CSV files containing employee project assignments and identifies which employees worked together on projects for the longest duration. The system is built with:

1. **Backend API**: .NET Core Web API that processes CSV files and calculates employee pair data
- ASP.NET Core backend (.NET 9)
- C# 13.0
2. **Frontend UI**: React TypeScript application that provides the user interface
- React Typescript
- TailwindCSS

## API Endpoints

### Upload Employee Data
#### POST /api/pairs/upload
- **Consumes**: multipart/form-data
- **Parameters**: file (CSV file)
- **Returns**: JSON object with pairs of employees who worked together the longest on each project

### Running the Application
1. Clone the repository
2. Open the backend solution in Visual Studio
3. Build the solution
4. Run the application
5. Open frontend solution in VSCode or any other IDE.
6. Run npm install and npm run dev
7. Frontend configuration
   - Open src/env.ts and modify the API_URL value to point to your API endpoint;

### Usage

1. Once the applications are launched, click "Choose CSV File" to select a CSV file with employee project data
3. The file should have columns for employee IDs, project IDs, and date ranges
4. Click "Upload" to process the file
5. View the results showing employee pairs and their total collaboration time
6. The pair with the longest collaboration time will be highlighted at the top
