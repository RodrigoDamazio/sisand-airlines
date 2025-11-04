import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { CheckoutComponent } from './pages/checkout/checkout.component';
import { authGuard } from './core/guards/auth.guard';

export const APP_ROUTES: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'checkout', component: CheckoutComponent, canActivate: [authGuard] },
  {path: 'minhas-reservas', loadComponent: () => import('./pages/minhas_reservas/minhas_reservas.component')
                          .then(m => m.MinhasReservasComponent), canActivate: [authGuard]},
  { path: '**', redirectTo: '' }
];
