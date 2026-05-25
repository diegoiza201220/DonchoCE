import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ProductosComponent } from './components/productos/productos.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { MainComponent } from './components/main/main.component';
import { PedidosComponent } from './components/pedidos/pedidos.component';
import { CocinaComponent } from './components/cocina/cocina.component';
import { TableModule } from 'primeng/table';
import { MessagesModule } from 'primeng/messages';
import { ToastModule } from 'primeng/toast';
import { FormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CheckboxModule } from 'primeng/checkbox';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';
import { ConfirmationService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { BadgeModule } from 'primeng/badge';
import { TabViewModule } from 'primeng/tabview';
import { DropdownModule } from 'primeng/dropdown';
import { SplitterModule } from 'primeng/splitter';
import { LoginComponent } from './components/login/login.component';
import { RptComprasComponent } from './components/reportes/compras/rptcompras.component';
import { RptVentasComponent } from './components/reportes/ventas/rptventas.component';
import { CalendarModule } from 'primeng/calendar';
import { RptVentasproductosComponent } from './components/reportes/ventasproductos/rptventasproductos.component';
import { ChartModule } from 'primeng/chart';
import { ItemsComponent } from './components/items/items.component';
import { RptOrdenesComponent } from './components/reportes/ordenes/rptordenes.component';
import { ComprasComponent } from './components/compras/compras.component';
import { TwoDigitDecimaNumberDirective } from './util/directives/twodigitdecimalnumberdirective';
import { InventariosComponent } from './components/inventarios/inventarios.component';
import { RptInventariosComponent } from './components/reportes/inventarios/rptinventarios.component';
import { RptComprasVsInventariosComponent } from './components/reportes/comprasvsinventarios/rptcomprasvsinventarios.component';

import { AuthInterceptor } from './services/auth.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    ProductosComponent,
    NavbarComponent,
    MainComponent,
    PedidosComponent,
    CocinaComponent,
    LoginComponent,
    RptComprasComponent,
    RptVentasComponent,
    RptVentasproductosComponent,
    ItemsComponent,
    RptOrdenesComponent,
    ComprasComponent,
    TwoDigitDecimaNumberDirective,
    InventariosComponent,
    RptInventariosComponent,
    RptComprasVsInventariosComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,       // reemplaza Firebase SDK
    TableModule, MessagesModule, InputTextModule, InputNumberModule,
    ToastModule, FormsModule, BrowserAnimationsModule, BadgeModule,
    TabViewModule, ChartModule, CheckboxModule, ToolbarModule, ButtonModule,
    DialogModule, ConfirmDialogModule, DropdownModule, SplitterModule, CalendarModule
  ],
  providers: [
    ConfirmationService,
    // Interceptor que adjunta el JWT Bearer token a cada petición HTTP
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
