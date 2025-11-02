import { Component, signal } from '@angular/core';
// import { RouterOutlet } from '@angular/router';

@Component({
  standalone: false, // ✅ Required for module-based bootstrapping

  selector: 'app-root',
  //imports: [RouterOutlet],      // ❌ Only used in standalone: true

  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('devpulse-ui');
}
