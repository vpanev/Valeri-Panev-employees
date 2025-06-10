import { useState, useRef } from "react";
import axios from "axios";
import env from "../env";
import type { EmployeePair } from "../types/EmployeePair";
import type { ApiResponse } from "../types/ApiResponse";

function EmployeePairs() {
	const [pairs, setPairs] = useState<EmployeePair[]>([]);
	const [selectedFile, setSelectedFile] = useState<File | null>(null);
	const [isLoading, setIsLoading] = useState(false);
	const [errorMessage, setErrorMessage] = useState<string | null>(null);
	const fileInputRef = useRef<HTMLInputElement>(null);
	const upload = async () => {
		if (!selectedFile) return;

		setPairs([]);
		setErrorMessage(null);
		setIsLoading(true);

		try {
			const form = new FormData();
			form.append("file", selectedFile);
			const response = await axios.post<ApiResponse>(`${env.API_URL}${env.API_ENDPOINTS.UPLOAD}`, form, {
				headers: { "Content-Type": "multipart/form-data" },
			});

			if (response.data.success) {
				setPairs(response.data.data);
			} else {
				setErrorMessage(response.data.errors.join(". "));
			}
		} catch (error) {
			console.error("Error uploading file:", error);
			setErrorMessage("Failed to upload file. Please try again.");
		} finally {
			setIsLoading(false);
		}
	};

	const longestWorkingPair =
		pairs.length > 0 ? pairs.reduce((prev, current) => (prev.totalWorkDays > current.totalWorkDays ? prev : current)) : null;

	return (
		<div className="max-w-5xl w-full mx-auto bg-white shadow-lg rounded-lg p-8 mt-8">
			<h1 className="text-2xl font-bold text-gray-800 mb-6 text-center">Employee Pairs</h1>
			<div className="flex items-center gap-4 mb-6 flex-col">
				<div className="flex-grow flex items-center w-full gap-10">
					<input
						type="file"
						ref={fileInputRef}
						accept=".csv"
						className="hidden"
						onChange={(e) => {
							const file = e.target.files?.[0];
							if (file) {
								if (file.name.toLowerCase().endsWith(".csv")) {
									setSelectedFile(file);
									setErrorMessage(null);
								} else {
									setSelectedFile(null);
									setErrorMessage("Please select a CSV file only.");
								}
							} else {
								setSelectedFile(null);
								setErrorMessage(null);
							}
						}}
					/>
					<button
						onClick={() => fileInputRef.current?.click()}
						className="px-4 cursor-pointer py-2 rounded-md bg-gray-200 text-gray-700 hover:bg-gray-300 text-sm font-medium w-fit text-left"
					>
						Choose CSV File
					</button>
					<button
						onClick={upload}
						disabled={!selectedFile || isLoading}
						className={`px-4 py-2 rounded-md text-white text-sm font-medium  ${
							!selectedFile || isLoading ? "bg-gray-400 cursor-not-allowed" : "bg-blue-600 hover:bg-blue-700 cursor-pointer"
						}`}
					>
						{isLoading ? "Uploading..." : "Upload"}
					</button>
				</div>
				{selectedFile && (
					<div className="mb-4 px-3 py-2 bg-gray-50 border border-gray-200 rounded text-sm mr-auto flex items-center gap-2">
						<span>
							Selected file: <span className="font-medium">{selectedFile?.name}</span>
						</span>
						<button
							onClick={() => {
								setSelectedFile(null);
								setPairs([]);
								if (fileInputRef.current) {
									fileInputRef.current.value = "";
								}
							}}
							className="ml-2 text-gray-500 hover:text-red-400 focus:outline-none"
							title="Remove file"
						>
							<svg
								xmlns="http://www.w3.org/2000/svg"
								className="h-4 w-4 cursor-pointer"
								fill="none"
								viewBox="0 0 24 24"
								stroke="currentColor"
							>
								<path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
							</svg>
						</button>
					</div>
				)}
			</div>
			{errorMessage && <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded mb-6">{errorMessage}</div>}
			{longestWorkingPair && (
				<h2 className="text-xl font-semibold mb-4">
					Longest-working pair:
					<span className="text-blue-600">
						{longestWorkingPair.employeeID1} &amp; {longestWorkingPair.employeeID2} ({longestWorkingPair.totalWorkDays} days)
					</span>
				</h2>
			)}
			{pairs.length > 0 && (
				<div className="overflow-x-auto">
					<table className="min-w-full bg-white shadow rounded-lg">
						<thead className="bg-gray-50">
							<tr>
								<th className="px-4 py-2 text-left text-sm font-medium text-gray-700">Employee ID #1</th>
								<th className="px-4 py-2 text-left text-sm font-medium text-gray-700">Employee ID #2</th>
								<th className="px-4 py-2 text-left text-sm font-medium text-gray-700">Project ID</th>
								<th className="px-4 py-2 text-left text-sm font-medium text-gray-700">Days Worked</th>
							</tr>
						</thead>
						<tbody>
							{pairs.map((pair, i) => (
								<tr key={i} className={i % 2 === 0 ? "bg-white" : "bg-gray-50"}>
									<td className="px-4 py-2">{pair.employeeID1}</td>
									<td className="px-4 py-2">{pair.employeeID2}</td>
									<td className="px-4 py-2">{pair.projectID}</td>
									<td className="px-4 py-2">{pair.totalWorkDays}</td>
								</tr>
							))}
						</tbody>
					</table>
				</div>
			)}
		</div>
	);
}

export default EmployeePairs;
