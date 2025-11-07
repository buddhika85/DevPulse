import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

// Marks this class as injectable so Angular can use it as an HTTP interceptor
@Injectable()
export class ApimSubscriptionInterceptor implements HttpInterceptor {
  // This method intercepts every outgoing HTTP request
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const key = (window as any).__env?.NG_APP_APIM_SUBSCRIPTION_KEY;

    if (key) {
      // Clone the original request and add the APIM subscription key header
      const cloned = req.clone({
        setHeaders: {
          // This header is required by Azure API Management to authorize requests
          'Ocp-Apim-Subscription-Key': key, // ✅ Use runtime-injected key
        },
      });
      // Pass the modified request to the next handler in the chain
      return next.handle(cloned);
    }

    console.warn('APIM subscription key is missing from window.__env');
    return next.handle(req);
  }
}
