import { Component, inject } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import Item from '@interfaces/item.interface';
import { AuthService } from '@services/auth.service';
import { ItemsService } from '@services/items.service';
import { ComprasService } from '@services/compras.service';
import { BaseComponent } from '@util/base.component';
import { InventariosService } from '@services/inventarios.service';
import { Router } from '@angular/router';
import { LoggerService } from '@services/logger.service';  

@Component({
  selector: 'app-inventarios',
  templateUrl: './inventarios.component.html',
  styleUrls: ['./inventarios.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class InventariosComponent extends BaseComponent {

  private readonly itemService = inject(ItemsService);
  private readonly messageService = inject(MessageService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly compraService = inject(ComprasService);
  private readonly router = inject(Router);
  public override authService = inject(AuthService);
  public override logger: LoggerService = inject(LoggerService);

  constructor() {
    super();  
  }

  item!: Item;
  litems: any;
  clonedItem: { [s: string]: Item } = {};
  selectedItems!: any[];
  submitted!: boolean;
  itemDialogo!: boolean;

  inventario: any = {};

  ngOnInit(): void {
    this.inventario = {};
    this.inventario.fecha = new Date();
    //this.inventario.total = 0.00;
    this.litems = [];
    this.getItemsPromise();
    this.submitted = false;
  }

  getItemsPromise(): void {
    this.itemService.getItemsPromise().then(items => {
      items.forEach(element => {
        let dat : any = {};
        dat = element;
        dat.existencia = 0;
        this.litems.push(dat);
      });
    })
    
  }

  grabarInventario() {
    this.submitted = true;
    this.inventario.secuencial = 0;
    this.inventario.fechainteger = this.fechaToInteger(this.inventario.fecha);
    this.inventario.usuario = this.authService.userEmail;
    this.inventario.items = [];
    
    this.litems.forEach((ele: any) => {
      if (ele.existencia>0){
        let i : any = {};
        i.id = ele.id;
        i.nombre = ele.nombre;
        i.unidad = ele.unidad;
        i.existencia = ele.existencia;
        this.inventario.items.push(i);
      }
    })

    //this.inventarioService.addCompra(this.inventario);
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'El inventario ha sido registrado' });
  }

  applyFilterGlobal($event: any, stringVal: any, dt: any) {
    dt.filterGlobal(($event.target as HTMLInputElement).value, 'contains');
  }

  navigateTo(){
    this.router.navigateByUrl('main');
  }
}
