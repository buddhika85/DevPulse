import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-error-status-code',
  imports: [RouterLink],
  templateUrl: './error-status-code.html',
  styleUrl: './error-status-code.scss',
})
export class ErrorStatusCode implements OnInit {
  private activatedRoute = inject(ActivatedRoute);
  errorStatusCode!: number;
  title = '';
  message = '';
  details = '';

  ngOnInit(): void {
    // The + - unary plus operator - converts a string into a number.
    // The ! (Non‑Null Assertion Operator) - trust me, this value is not null or undefined.

    this.errorStatusCode =
      +this.activatedRoute.snapshot.paramMap.get('status')!;

    const map = this.describeStatus(this.errorStatusCode);
    this.title = map.title;
    this.message = map.message;
  }

  reload(): void {
    location.reload();
  }

  private describeStatus(code: number) {
    switch (code) {
      case 401:
        return {
          title: 'Unauthorized',
          message: 'Please sign in to continue.',
        };
      case 403:
        return {
          title: 'Forbidden',
          message: 'You do not have permission to view this resource.',
        };
      case 404:
        return {
          title: 'Not found',
          message: 'The requested resource could not be found.',
        };
      case 500:
        return {
          title: 'Server error',
          message: 'Something went wrong on server side.',
        };
      default:
        return {
          title: `Error ${code}`,
          message: 'An unexpected error occurred.',
        };
    }
  }
}
