import { Component, OnInit } from '@angular/core';
import {HttpClient} from '@angular/common/http'

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  private url: string = "";


  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    // this.http.get('http://127.0.0.1:4433/self-service/login/browser').subscribe((data: any) => {
    //   console.log(data);
    //   this.url = data.ui.action;
    // });
  }


  login(){

    console.log("Calling API...");
    //, responseType: "text"
    this.http.get('http://127.0.0.1:4455/api/secure', { withCredentials: true }).subscribe((data: any) => {
      console.log(data);
    });


  }

}
