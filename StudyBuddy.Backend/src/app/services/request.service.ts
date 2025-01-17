import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EventEmitter, Injectable, Output } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AuthorizationService } from './authorization.service';
import { LoggingService } from './loging.service';
import { Request } from '../model/request';
import { RequestList } from '../model/request.list';

@Injectable({
  providedIn: 'root'
})
export class RequestService {
  private url = environment.api_url;
  @Output() changed = new EventEmitter();

  constructor(
    private logger: LoggingService,
    private http: HttpClient,
    private auth: AuthorizationService) { }

  async getAll(page: number = -1): Promise<RequestList> {
    var query = {};

    if (page != -1) {
      query['start'] = (page - 1) * 10;
      query['count'] = 10;
    }

    let result = await this.http.get(this.url + "Request",
      {
        params: query,
        headers: new HttpHeaders({ Authorization: this.auth.getToken() })
      }).toPromise();

    return RequestList.fromResult(result);
  }

  async remove(id: number) {
    if (!this.auth.isLoggedIn())
      return;

    let path = this.url + "Request/" + id;
    this.logger.debug("Removing Request from " + path);
    let result = await this.http.delete(path,
      {
        headers: new HttpHeaders({ Authorization: this.auth.getToken() })
      }).toPromise();

    this.changed.emit();
  }

  async byId(id: number): Promise<Request> {
    let path = this.url + "Request/" + id;
    this.logger.debug("Loading Request from " + path);
    let result = await this.http.get(path,
      {
        headers: new HttpHeaders({ Authorization: this.auth.getToken() })
      }).toPromise();

    if (result == null) {
      this.logger.debug("Object not found!");
      return null;
    }
    else
      return Request.fromApi(result);
  }

  async save(obj: Request) {
    if (!this.auth.isLoggedIn())
      return null;

    let data = obj.toApi();
    this.logger.debug("Saving new Request");
    let result = await this.http.post(this.url + "Request", data,
      {
        headers: new HttpHeaders({ Authorization: this.auth.getToken() })
      }).toPromise();

    obj.id = result["id"];
    this.changed.emit();
  }

  async accept(id: number) {
    if (!this.auth.isLoggedIn())
      return null;

    this.logger.debug("Accepting Request " + id);
    let result = await this.http.post(this.url + "Request/Accept/" + id, {},
      {
        headers: new HttpHeaders({ Authorization: this.auth.getToken() })
      }).toPromise();

    this.changed.emit();
  }
}