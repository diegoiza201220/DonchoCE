import { Component, OnInit } from '@angular/core';
import { MessageService, SelectItem, ConfirmationService } from 'primeng/api';
import Producto from 'src/app/interfaces/productos.interface';
import { ProductosService } from 'src/app/services/productos.service';
import { LoggerService } from 'src/app/services/logger.service';
import { CatalogosService } from 'src/app/services/catalogos.service';
import { OrdenesService } from 'src/app/services/ordenes.service';
import { BaseComponent } from 'src/app/util/base.component';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-productos',
  templateUrl: './productos.component.html',
  styleUrls: ['./productos.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class ProductosComponent extends BaseComponent implements OnInit {

  producto!: any;

  lproductos: Producto[] = [];
  ltarifas: any[] = [];

  clonedProductos: { [s: string]: Producto } = {};

  selectedProductos!: Producto[];

  submitted!: boolean;

  productoDialogo!: boolean;
  impuestoPorcentaje = 0;
  codigoIva = 0;
  idCatDetalleIva = 0;

  lgrupo: SelectItem[] = [
    { label: 'CHOCLO', value: 'CHOCLO' },
    { label: 'CHOCHO', value: 'CHOCHO' },
    { label: 'PORCIONES', value: 'PORCIONES' },
    { label: 'BEBIDAS', value: 'BEBIDAS' },
    { label: 'OTROS', value: 'OTROS' }
  ];

  constructor(private readonly productosService: ProductosService,
    private readonly messageService: MessageService,
    private readonly confirmationService: ConfirmationService,
    private readonly ordenesService: OrdenesService,
    public override authService: AuthService,
    public override logger: LoggerService,
    private readonly catalogosService: CatalogosService) {
    super(authService, logger);
  }

  ngOnInit(): void {
    this.getProductosPromise();
    this.getDatosPedido();
  }

  openNew() {
    this.producto = { nombre: '', valor: 0, grupo: '', activo: true, ordenaparicion: 0, 
      pedidoacocina: false , ivaValor: 0, 
      valorTotal: 0, codigoIva: this.idCatDetalleIva, valorDoncho: 0 };
    this.submitted = false;
    this.productoDialogo = true;
  }

  hideDialog() {
    this.productoDialogo = false;
    this.submitted = false;
  }

  saveProduct() {
    this.submitted = true;
    this.addProducto();
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Producto creado' });
    this.productoDialogo = false;
  }


  getDatosPedido() {
    this.ordenesService.getDatosPedido().then(data => {
      this.impuestoPorcentaje = data.impuestoPorcentaje;
      this.codigoIva = data.codigoIva;
      this.idCatDetalleIva = data.idCatDetalleIva;
    })
  }

  getProductosPromise(): void {
    this.lproductos = [];
    this.productosService.getProductosPromise().then(data => {
      data.productos.forEach((producto: Producto) => {
        this.lproductos.push(producto);
      });
    })
  }

  getCatalogosPromise(): void {
    this.ltarifas = [];
    this.catalogosService.getCatalogosPromise({ ValorString1: 'SRI_TABLA_17_TARIFA_IVA' }).then(resp => {
      this.ltarifas = resp;
    });
  }


  async addProducto() {
    this.lproductos = [];
    this.productosService.addProducto(this.producto)
      .subscribe(response => {
        this.logger.log(response);
        this.getProductosPromise();
      })
  }

  async deleteProducto(producto: Producto) {
    this.lproductos = [];
    this.productosService.deleteProducto(producto)
      .subscribe(response => {
        this.logger.log(response);
        this.getProductosPromise();
      })
  }

  async updateProducto(producto: Producto) {
    this.lproductos = [];
    this.productosService.updateProducto(producto)
      .subscribe(response => {
        this.logger.log(response);
        this.getProductosPromise();
      });
  }

  onRowEditInit(producto: Producto) {
    this.clonedProductos[producto.id as string] = { ...producto };
  }

  onRowEditSave(producto: Producto) {
    this.updateProducto(producto);
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Producto actualizado' });
  }

  onRowEditCancel(producto: Producto, index: number) {
    this.lproductos[index] = this.clonedProductos[producto.id as string];
    delete this.clonedProductos[producto.id as string];
    this.messageService.add({ severity: 'warn', summary: 'Ops!!', detail: 'Producto no fue actualizado!' });
  }

  deleteSelectedProducts() {
    this.confirmationService.confirm({
      message: '¿Estás seguro de eliminar los registros seleccionados?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.selectedProductos.forEach(element => {
          this.deleteProducto(element);
        });
        this.messageService.add({ severity: 'success', summary: '¡Muy bien!', detail: 'Productos han sido eliminados', life: 3000 });
        this.getProductosPromise();
      }
    });
  }

  calcularIVA(value: any) {
    const p = value;
    p.ivaValor = this.redondear(p.valor * (this.impuestoPorcentaje / 100), 2);
    p.valorTotal = this.redondear(Number(p.valor) + Number(p.ivaValor), 2);
  }
}
