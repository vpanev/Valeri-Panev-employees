import { Employee } from './Employee';

export interface FileEmployeeUploadResponse {
  employees: {
    csvWorkedTogetherCollection: Employee[];
  };
}
