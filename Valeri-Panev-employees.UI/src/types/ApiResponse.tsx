import type { EmployeePair } from "./EmployeePair";

export type ApiResponse = {
    data: EmployeePair[];
    errors: string[];
    success: boolean;
};
