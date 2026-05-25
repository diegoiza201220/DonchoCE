import { Component } from '@angular/core';
import { MessageService, SelectItem, ConfirmationService } from 'primeng/api';
import Item from 'src/app/interfaces/item.interface';
import { AuthService } from 'src/app/services/auth.service';
import { ItemsService } from 'src/app/services/items.service';
import { BaseComponent } from 'src/app/util/base.component';
import { ComprasService } from 'src/app/services/compras.service';
import { Router } from '@angular/router';
import { LoggerService } from 'src/app/services/logger.service';

@Component({
  selector: 'app-compras',
  templateUrl: './compras.component.html',
  styleUrls: ['./compras.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class ComprasComponent extends BaseComponent {

  constructor(private itemService: ItemsService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private compraService: ComprasService,
    private router: Router,
    public override authService: AuthService,
    public override logger: LoggerService) {
    super(authService, logger);
  }

  item!: Item;
  litems: any;
  clonedItem: { [s: string]: Item } = {};
  selectedItems!: any[];
  submitted!: boolean;
  itemDialogo!: boolean;

  compra: any = {};

  ngOnInit(): void {
    this.compra = {};
    this.compra.total = 0;
    this.litems = [];
    this.getItemsPromise();
    this.submitted = false;
  }

  onBlurItem(item :any){
    item.preciototal = this.redondear(item.preciounitario * item.cantidad, 2);
    this.calcularTotal();
  }

  calcularTotal(){
    let total = 0;
    this.litems.forEach((x: any)=>{
      if (x.cantidad>0){
        total += x.preciototal;
      }
    })
    this.compra.total = this.redondear(total,2);
  }

  hideDialog() {
    this.itemDialogo = false;
    this.submitted = false;
  }

  getItemsPromise(): void {
    this.itemService.getItemsPromise().then(items => {
      items.forEach(element => {
        let dat : any = {};
        dat = element;
        dat.cantidad = 0;
        dat.preciounitario = 0;
        dat.preciototal = 0;
        this.litems.push(dat);
      });
    })
    
  }

  grabarCompra() {
    this.submitted = true;
    this.compra.secuencial = 0;
    this.compra.fechareal = new Date();
    this.compra.fechainteger = this.fechaToInteger(this.compra.fecha);
    this.compra.usuario = this.authService.userEmail;
    this.compra.items = [];
    
    this.litems.forEach((ele: any) => {
      if (ele.cantidad>0){
        let i : any = {};
        i.id = ele.id;
        i.nombre = ele.nombre;
        i.cantidad = ele.cantidad;
        i.preciounitario = ele.preciounitario;
        i.preciototal = ele.preciototal;
        i.unidad = ele.unidad;
        this.compra.items.push(i);
      }
    })

    this.compraService.addCompra(this.compra);
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'La compra ha sido registrada' });
  }

  applyFilterGlobal($event: any, stringVal: any, dt: any) {
    dt.filterGlobal(($event.target as HTMLInputElement).value, 'contains');
  }

  navigateTo(){
    this.router.navigateByUrl('main');
  }
}
