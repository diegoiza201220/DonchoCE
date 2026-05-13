import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductosComponent } from './components/productos/productos.component';
import { MainComponent } from './components/main/main.component';
import { PedidosComponent } from './components/pedidos/pedidos.component';
import { CocinaComponent } from './components/cocina/cocina.component';
import { ComprasComponent } from './components/compras/compras.component';
import { ItemsComponent } from './components/items/items.component';
import { RptOrdenesComponent } from './components/reportes/ordenes/rptordenes.component';
import { RptComprasComponent } from './components/reportes/compras/rptcompras.component';
import { RptVentasComponent } from './components/reportes/ventas/rptventas.component';
import { RptVentasproductosComponent } from './components/reportes/ventasproductos/rptventasproductos.component';
import { LoginComponent } from './components/login/login.component';
import { AuthGuard } from './guards/auth.guard';
import { InventariosComponent } from './components/inventarios/inventarios.component';
import { RptInventariosComponent } from './components/reportes/inventarios/rptinventarios.component';
import { RptComprasVsInventariosComponent } from './components/reportes/comprasvsinventarios/rptcomprasvsinventarios.component';

const routes: Routes = [
  {path: '', redirectTo: '/login',pathMatch: 'full'},
  {path: 'login', component: LoginComponent},
  {path: 'main', component: MainComponent, canActivate: [AuthGuard] },
  {path: 'productos', component: ProductosComponent, canActivate: [AuthGuard]},
  {path: 'items', component: ItemsComponent, canActivate: [AuthGuard]},
  {path: 'compras', component: ComprasComponent, canActivate: [AuthGuard]},
  {path: 'inventarios', component: InventariosComponent, canActivate: [AuthGuard]},
  {path: 'pedidos', component: PedidosComponent, canActivate: [AuthGuard]},
  {path: 'cocina', component: CocinaComponent, canActivate: [AuthGuard]},
  {path: 'rptordenes', component: RptOrdenesComponent, canActivate: [AuthGuard]},
  {path: 'rptcompras', component: RptComprasComponent, canActivate: [AuthGuard]},
  {path: 'rptventas', component: RptVentasComponent, canActivate: [AuthGuard]},
  {path: 'rptventasproductos', component: RptVentasproductosComponent, canActivate: [AuthGuard]},
  {path: 'rptinventarios', component: RptInventariosComponent, canActivate: [AuthGuard]},
  {path: 'rptcomprasvsinventarios', component: RptComprasVsInventariosComponent, canActivate: [AuthGuard]},
  {path: '**', component: LoginComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
