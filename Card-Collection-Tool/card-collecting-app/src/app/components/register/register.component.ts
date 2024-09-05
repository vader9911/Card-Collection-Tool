import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule
  ],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  registrationSuccess: boolean = false; // Flag to indicate registration status
  registrationError: string = ''; // Holds any error message from registration

  constructor(private fb: FormBuilder, private authService: AuthService) {
    // Initialize the form with form controls and validators
    this.registerForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    });
  }

  onSubmit() {
    const { email, password, confirmPassword } = this.registerForm.value;

    // Check if passwords match
    if (password !== confirmPassword) {
      this.registrationError = 'Passwords do not match';
      return;
    }

    // Call the AuthService to register the user
    this.authService.register(email, password).subscribe(
      response => {
        this.registrationSuccess = true; // Set success flag
        this.registrationError = ''; // Clear any previous errors
        console.log('Registration successful:', response);
      },
      error => {
        this.registrationSuccess = false; // Clear success flag
        this.registrationError = 'Registration failed. Please try again.'; // Set error message
        console.error('Registration failed:', error);
      }
    );
  }
}
