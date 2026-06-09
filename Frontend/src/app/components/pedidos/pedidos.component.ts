import { Component, ElementRef, ViewChild, OnInit } from '@angular/core';
import Producto from 'src/app/interfaces/productos.interface';
import { ProductosService } from 'src/app/services/productos.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { SecuenciaService } from 'src/app/services/secuencia.service';
import { OrdenesService } from 'src/app/services/ordenes.service';
import { ClientesService } from 'src/app/services/clientes.service';
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

  impuestoPorcentaje = 0;
  codigoIva = 0;
  pago: number = 0;
  cambio: number = 0;
  loading: boolean = false;
  fechainteger: number = 0;
  activeIndex: number = 0;
  searchingCliente: boolean = false;
  clienteEncontrado: boolean = false;

  cliente: any = {};
  clienteDialog = false;

  lFacturar: any[] = [
    { label: 'Si', value: true },
    { label: 'No', value: false }
  ];

  @ViewChild('efectivorecibido') input: ElementRef | undefined;

  constructor(private readonly productosService: ProductosService,
    private readonly messageService: MessageService,
    private readonly confirmationService: ConfirmationService,
    private readonly secuenciaService: SecuenciaService,
    private readonly ordenesService: OrdenesService,
    private readonly ordenesCocinaService: OrdenescocinaService,
    private readonly clientesService: ClientesService,
    private readonly router: Router,
    private readonly datePipe: DatePipe,
    public override authService: AuthService,
    public override logger: LoggerService
  ) {
    super(authService, logger);
  }

  ngOnInit(): void {
    this.configurarPedido();
    this.getProductosPromise();
    this.getSecuenciaPromise();
    this.getDatosPedido();
  }

  configurarPedido() {
    this.pedido = {};
    this.pedido.esFactura = false;
    this.cliente = {};
  }

  getDatosPedido() {
    this.ordenesService.getDatosPedido().then(data => {
      this.impuestoPorcentaje = data.impuestoPorcentaje;
      this.codigoIva = data.codigoIva;
      this.lproductos.forEach((producto: Producto) => {
        producto.valor = this.redondear(producto.valor + (producto.valor * this.impuestoPorcentaje / 100), 2);
      });
    })
  }

  getProductosPromise(): void {
    this.lproductos = [];
    this.productosService.getProductosPromise().then(data => {
      data.productos.forEach((producto: Producto) => {
        producto.valorsiniva = producto.valor;
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

  // getProductosObserver(): void {
  //   this.productosService.getProductosObservable().subscribe(productos => {
  //     this.lproductos = productos;
  //   })
  // }

  // getSecuenciaObserver(): void {
  //   this.secuenciaService.getSecuenciaObservable().subscribe(secuencia => {
  //     let d = new Date();
  //     this.fechainteger = this.fechaToInteger(d);
  //     this.lsecuencia = secuencia[0];
  //     if (this.lsecuencia.fecha !== this.fechainteger) {
  //       this.lsecuencia.fecha = this.fechainteger;
  //       this.lsecuencia.secuencia = 1;
  //     }
  //   })
  // }

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
    this.pedido.TotalSinImpuestos = this.pedido.FacDetalleOrdens.reduce((sum: any, current: { PrecioTotal: any; }) => sum + current.PrecioTotal, 0);
    this.pedido.ImpuestoValor = this.redondear(this.pedido.TotalSinImpuestos * this.impuestoPorcentaje / 100, 2);
    this.pedido.TotalOrden = this.redondear(this.pedido.TotalSinImpuestos + this.pedido.ImpuestoValor, 2);
  }

  calcularDetalles(lista: any[]) {
    lista.forEach(element => {
      this.pedido.FacDetalleOrdens.push(
        {
          ProductoId: element.id,
          Cantidad: element.badge,
          Nombre: element.nombre,
          PrecioUnitario: element.valorsiniva,
          ImpuestoCodigo: 2,
          ImpuestoCodigoPorcentaje: this.codigoIva,
          ImpuestoTarifa: this.impuestoPorcentaje,
          ImpuestoValor: this.redondear(element.valorsiniva * this.impuestoPorcentaje / 100, 2),
          PrecioTotal: element.badge * element.valorsiniva,
          PedidoACocina: element.pedidoacocina,
          ValorIva: this.redondear(element.valorsiniva * this.impuestoPorcentaje / 100, 2)
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

    if (this.pedido.esFactura === null || this.pedido.esFactura === undefined) {
      this.messageService.add({ severity: 'warn', summary: 'Ops!! ', detail: '¡Debe indicar si el pedido es para factura o no!' });
      return;
    }

    if (this.pedido.FacDetalleOrdens === undefined || this.pedido.FacDetalleOrdens.length === 0) {
      this.messageService.add({ severity: 'warn', summary: 'Ops!! ', detail: '¡Debe agregar al menos un producto al pedido!' });
      return;
    }

    if (!this.pedido.esFactura) {
      this.cliente.id = 1;
      this.cliente.nombre = 'Consumidor final';
      this.cliente.apellido = '';
      this.cliente.cedulaRuc = '9999999999';
      this.cliente.direccion = 'NA';
      this.cliente.telefono_celular = '0000000000';
      this.cliente.usuarioRegistro = this.authService.userEmail;
    } else if (this.pedido.esFactura && this.cliente.id === undefined) {
      this.messageService.add({ severity: 'warn', summary: 'Ops!! ', detail: '¡Debe ingresar los datos del cliente para facturar!' });
      return;
    } 

    let d = new Date();

    this.pedido.Clienteid = this.cliente.id === null ? 1 : this.cliente.id;
    this.pedido.UsuarioRegistro = this.authService.userEmail;
    this.pedido.Secuencial = this.lsecuencia.secuencia;
    this.pedido.TipoPago = this.selectedFP;
    this.pedido.Fecha = d;
    this.pedido.FechaInteger = this.fechainteger;
    this.pedido.ValorIva = this.codigoIva;
    this.pedido.NumeroFactura = '000';
    this.pedido.DocumentoPago = '';
    this.pedido.ImpuestoBaseImponible = this.pedido.TotalSinImpuestos;
    this.pedido.ImpuestoCodigo = 2;
    this.pedido.ImpuestoCodigoPorcentaje = this.codigoIva;
    this.pedido.Cliente = this.cliente;
    this.pedido.ImpuestoPorcentaje = this.impuestoPorcentaje;
    this.logger.log(this.pedido);
    this.loading = true;
    this.ordenesService.addOrden(this.pedido).then((data) => {
      this.cleanPedidos();
      this.backToSeleccion();
      this.loading = false;
      this.messageService.add({
        severity: 'success',
        summary: '¡Éxito!',
        detail: 'Pedido ' + data.secuencial + ' creadook',
        life: 3000 // Duración en milisegundos (3 segundos)
      });
    }, (error) => {
      this.messageService.add({ severity: 'error', summary: 'Ops!! ', detail: 'Error al crear el pedido' });
      this.loading = false;
    });
    super.actualizarTotalesPedidos();
  }

  cleanPedidos() {
    this.configurarPedido();
    this.getProductosPromise();
    this.getSecuenciaPromise();
    this.getDatosPedido();
    this.lproductoschoclo.forEach(element => {
      element.badge = '0';
    });
    this.lproductoschocho.forEach(element => {
      element.badge = '0';
    });
    this.lproductosporciones.forEach(element => {
      element.badge = '0';
    });
    this.lproductosbebidas.forEach(element => {
      element.badge = '0';
    });
    this.lproductosotros.forEach(element => {
      element.badge = '0';
    });
  }

  openClienteDialog() {
    this.clienteEncontrado = false;
    this.clienteDialog = true;
  }

  hideDialogCliente() {
    this.clienteDialog = false;
    this.cliente = {};
    this.clienteEncontrado = false;
  }

  saveCliente() {
    if (this.clienteEncontrado) {
      this.clienteDialog = false;
      return;
    }
    this.cliente.usuarioRegistro = this.authService.userEmail;
    this.clientesService.addItem(this.cliente).then(data => {
      this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Cliente creado' });
      this.clienteDialog = false;
      this.cliente.id = data.id;
      this.pedido.Clienteid = this.cliente.id;
    }).catch((error) => {
      this.messageService.add({ severity: 'error', summary: 'Ops!! ', detail: 'Error al crear el cliente' });
    });
  }

  searchCliente() {
    this.searchingCliente = true;
    this.clientesService.getClientePromiseByCedulaRuc(this.cliente.cedulaRuc).then(data => {
      this.cliente.nombre = data.nombre;
      this.cliente.apellido = data.apellido;
      this.cliente.direccion = data.direccion;
      this.cliente.telefono_celular = data.telefonoCelular;
      this.cliente.email = data.email;
      this.cliente.fecha_cumpleanios = data.fechaCumpleanios;
      this.cliente.id = data.id;
      this.searchingCliente = false;
      this.clienteEncontrado = true;
      this.cliente.usuarioRegistro = this.authService.userEmail;
      this.messageService.add({ severity: 'success', summary: '¡Muy bien!', detail: '¡El cliente fue encontrado!' });
    }).catch(error => {
      error.status === 404 ? this.messageService.add({ severity: 'warn', summary: 'Ops!!', detail: '¡El cliente no fue encontrado!' }) : this.messageService.add({ severity: 'error', summary: 'Ops!!', detail: 'Error al buscar el cliente!' });
      this.searchingCliente = false;
    });
  }
}

