import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { FileEmployeeUploadResponse } from '../models/FileEmployeeUploadResponse';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
  title = 'EmployeePairsUI';
  selectedFile: File;
  baseurl = 'https://localhost:7192/upload-file';
  uploadForm: FormGroup;
  displayedColumns: string[] = ['employeeID1', 'employeeID2', 'projectID', 'workedTimeTogether'];
  dataSource: MatTableDataSource<any>;

  constructor(private formBuilder: FormBuilder, private httpClient: HttpClient, private renderer: Renderer2) { }

  ngOnInit() {
    this.uploadForm = this.formBuilder.group({
      file: ['']
    });
  }

  onFileSelected(event: any) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.uploadForm.get('file').setValue(file);
    }
  }

  async uploadFile() {
    const formData = new FormData();
    formData.append('file', this.uploadForm.get('file').value);

    await this.httpClient.post<any>(this.baseurl, formData).subscribe({
      next: (response: FileEmployeeUploadResponse) => {
        this.dataSource = new MatTableDataSource(response.employees.csvWorkedTogetherCollection);
      }
    });
  }
}
