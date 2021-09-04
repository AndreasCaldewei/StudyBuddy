import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthorizationService } from './services/authorization.service';

@Injectable({
  providedIn: 'root'
})
export class RouteGuardService implements CanActivate {

  constructor(
    private service:AuthorizationService,
    private router:Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    if (!this.service.isLoggedIn()) {
      this.router.navigate(['/login']);
      return false;
    } else {
      let user = this.service.getUser();
      if (user.isTeacher()) {
        if (state.url == "/studyprogram" || 
            state.url == "/term" ||
            state.url == "/user" ||
            state.url == "/team")
            this.router.navigate(['/challenge']);
      }
    }

    return true;
  }
}