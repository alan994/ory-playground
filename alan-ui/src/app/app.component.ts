import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'alan-ui';

  /**
   *
   */
  constructor(private http: HttpClient) {


  }

  callSecureApiendpoint() {
    this.http.get("http://127.0.0.1:4455/api/cookie/secure", { withCredentials: true }).subscribe((data) => console.log(data));
  }
}
