import { Component, ElementRef, ViewChild, OnInit } from '@angular/core';
import Producto from 'src/app/interfaces/productos.interface';
import { ProductosService } from 'src/app/services/productos.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { SecuenciaService } from 'src/app/services/secuencia.service';
import { OrdenesService } from 'src/app/services/ordenes.service';
import { Router } from '@angular/router';
import { OrdenescocinaService } from 'src/app/services/ordenescocina.service';
import Secuencia from 'src/app/interfaces/secuencia.interface';
import { BaseComponent } from 'src/app/util/base.component';
import { DatePipe } from '@angular/common';
import { AuthService } from 'src/app/services/auth.service';
import { LoggerService } from 'src/app/services/logger.service';

@Component({
  selector: 'app-pedidos',
  templateUrl: './pedidos.component.html',
  styleUrls: ['./pedidos.component.css'],
  providers: [MessageService, ConfirmationService, DatePipe]
})
export class PedidosComponent extends BaseComponent implements OnInit {
  lproductos: any[] = [];
  lproductoschoclo: any[] = [];
  lproductoschocho: any[] = [];
  lproductosporciones: any[] = [];
  lproductosbebidas: any[] = [];
  lproductosotros: any[] = [];

  lsecuencia: Secuencia = { secuencia: 0, id: '', fecha: 0 };
  pedido: any = {};
  lordencocina: any[] = [];
  mostrarCargar: boolean = true;
  selectedFP: string = "EF";

  pago: number = 0;
  cambio: number = 0;
  loading: boolean = false;
  fechainteger: number = 0;
  activeIndex: number = 0;

  @ViewChild('efectivorecibido') input: ElementRef | undefined;

  constructor(private readonly productosService: ProductosService,
    private readonly messageService: MessageService,
    private readonly confirmationService: ConfirmationService,
    private readonly secuenciaService: SecuenciaService,
    private readonly ordenesService: OrdenesService,
    private readonly ordenesCocinaService: OrdenescocinaService,
    private readonly router: Router,
    private readonly datePipe: DatePipe,
    public override authService: AuthService,
    public override logger: LoggerService
  ) {
    super(authService, logger);
  }

  ngOnInit(): void {
    this.pedido = {};
    this.getProductosPromise();
    this.getSecuenciaPromise();
  }

  getProductosPromise(): void {
    this.lproductos = [];
    this.productosService.getProductosPromise().then(data => {
      data.productos.forEach((producto: Producto) => {
        this.lproductos.push(producto);
      });
      this.fillGrupoProducto();
    })
  }

  getSecuenciaPromise(): void {
    this.secuenciaService.getSecuenciaPromise().then(data => {
      this.lsecuencia = data.facsecuenciadia[0];
      let d = new Date();
      this.fechainteger = this.fechaToInteger(d);
      if (this.lsecuencia.fecha !== this.fechainteger) {
        this.lsecuencia.fecha = this.fechainteger;
        this.lsecuencia.secuencia = 1;
      }
    })
  }

  getProductosObserver(): void {
    this.productosService.getProductosObservable().subscribe(productos => {
      this.lproductos = productos;
    })
  }

  getSecuenciaObserver(): void {
    this.secuenciaService.getSecuenciaObservable().subscribe(secuencia => {
      let d = new Date();
      this.fechainteger = this.fechaToInteger(d);
      this.lsecuencia = secuencia[0];
      if (this.lsecuencia.fecha !== this.fechainteger) {
        this.lsecuencia.fecha = this.fechainteger;
        this.lsecuencia.secuencia = 1;
      }
    })
  }

  fillGrupoProducto() {
    if (!this.mostrarCargar) {
      return;
    }

    this.lproductos.forEach(element => {
      element.badge = '0';
      switch (element.grupo) {
        case 'CHOCLO': {
          this.lproductoschoclo.push(element);
          break;
        }
        case "CHOCHO": {
          this.lproductoschocho.push(element);
          break;
        }
        case "PORCIONES": {
          this.lproductosporciones.push(element);
          break;
        }
        case "BEBIDAS": {
          this.lproductosbebidas.push(element);
          break;
        }
        case "OTROS": {
          this.lproductosotros.push(element);
          break;
        }
      }
    });
    this.mostrarCargar = false;
    // this.lproductoschoclo.sort((a, b) => (Number(a.ordenaparicion) < Number(b.ordenaparicion) ? -1 : 1));
    // this.lproductoschocho.sort((a, b) => (Number(a.ordenaparicion) < Number(b.ordenaparicion) ? -1 : 1));
    // this.lproductosporciones.sort((a, b) => (Number(a.ordenaparicion) < Number(b.ordenaparicion) ? -1 : 1));
    // this.lproductosbebidas.sort((a, b) => (Number(a.ordenaparicion) < Number(b.ordenaparicion) ? -1 : 1));
    // this.lproductosotros.sort((a, b) => (Number(a.ordenaparicion) < Number(b.ordenaparicion) ? -1 : 1));
    this.logger.log(this.mostrarCargar);
  }


  onClickProducto(_producto: Producto, operacion: string) {
    switch (_producto.grupo) {
      case 'CHOCLO': {
        this.procesarAccion(this.lproductoschoclo, operacion, _producto);
        break;
      }
      case "CHOCHO": {
        this.procesarAccion(this.lproductoschocho, operacion, _producto);
        break;
      }
      case "PORCIONES": {
        this.procesarAccion(this.lproductosporciones, operacion, _producto);
        break;
      }
      case "BEBIDAS": {
        this.procesarAccion(this.lproductosbebidas, operacion, _producto);
        break;
      }
      case "OTROS": {
        this.procesarAccion(this.lproductosotros, operacion, _producto);
        break;
      }
    }
  }

  onChangeTab(event: any) {
    this.activeIndex = event.index;
    switch (event.index) {
      case 1: {
        this.cargarDetalleOrden();
        break;
      }
      case 2: {
        break;
      }
    }
  }

  continueToResumen() {
    this.activeIndex = 1;
    this.cargarDetalleOrden();
  }

  backToSeleccion() {
    this.activeIndex = 0;
  }

  cargarDetalleOrden() {
    this.pedido.FacDetalleOrdens = [];
    this.calcularDetalles(this.lproductoschocho.filter(x => x.badge > 0));
    this.calcularDetalles(this.lproductoschoclo.filter(x => x.badge > 0));
    this.calcularDetalles(this.lproductosporciones.filter(x => x.badge > 0));
    this.calcularDetalles(this.lproductosbebidas.filter(x => x.badge > 0));
    this.calcularDetalles(this.lproductosotros.filter(x => x.badge > 0));
    this.pedido.TotalOrden = this.pedido.FacDetalleOrdens.reduce((sum: any, current: { PrecioTotal: any; }) => sum + current.PrecioTotal, 0);
  }

  calcularDetalles(lista: any[]) {
    lista.forEach(element => {
      this.pedido.FacDetalleOrdens.push(
        {
          ProductoId: element.id,
          Cantidad: element.badge,
          plato: element.nombre,
          PrecioUnitario: element.valor,
          ValorIva: 0,
          CodigoIva: '0',
          PrecioTotal: element.badge * element.valor,
          PedidoACocina: element.pedidoacocina
        });
    });
  }

  procesarAccion(lista: any[], operacion: string, producto: Producto) {
    let badge = lista.find(x => x.id == producto.id).badge;
    if (badge <= 0 && operacion === '-') return;
    operacion == '+' ? lista.find(x => x.id == producto.id).badge++ : lista.find(x => x.id == producto.id).badge--;
  }

  calcularCambio(value: any) {
    this.cambio = this.pago - this.pedido.TotalOrden;
  }

  grabarOrden() {

    if (this.activeIndex != 1) return;

    this.loading = true;
    setTimeout(() => {
      let d = new Date();
      this.pedido.Clienteid = 4;
      this.pedido.UsuarioRegistro = this.authService.userEmail;
      this.pedido.Secuencial = this.lsecuencia.secuencia;
      this.pedido.TipoPago = this.selectedFP;
      this.pedido.Fecha = d;
      this.pedido.FechaInteger = this.fechainteger;
      this.pedido.ValorIva = 0;
      this.pedido.NumeroFactura = '000';
      this.pedido.CodigoIva = '0';
      this.pedido.DocumentoPago = '';
      this.ordenesService.addOrden(this.pedido);
      this.loading = false;
    }, 100);

    this.router.navigateByUrl('main');
  }
}
