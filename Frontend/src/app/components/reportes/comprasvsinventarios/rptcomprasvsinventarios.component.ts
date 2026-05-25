import { Component } from '@angular/core';
//import * as FileSaver from 'file-saver';
import { BaseComponent } from 'src/app/util/base.component';
import { AuthService } from 'src/app/services/auth.service';
import Inventario from 'src/app/interfaces/inventario.interface';
import { InventariosService } from 'src/app/services/inventarios.service';
import { ComprasService } from 'src/app/services/compras.service';
import { ItemsService } from 'src/app/services/items.service';
import Compra from 'src/app/interfaces/compra.interface'
import Item from 'src/app/interfaces/item.interface';
import { LoggerService } from 'src/app/services/logger.service';

@Component({
  selector: 'app-rpt-comprasvsinventarios',
  templateUrl: './rptcomprasvsinventarios.component.html',
  styleUrls: ['./rptcomprasvsinventarios.component.css']
})
export class RptComprasVsInventariosComponent extends BaseComponent {
  [x: string]: any;

  d1 = new Date();
  d2 = new Date();
  lregistrosinventarios: Inventario[] = [];
  ldatainventarios: any[] = [];

  lregistrositems: Item[] = [];
  ldataitems: any[] = [];

  lregistroscompras: Compra[] = [];
  ldatacompras: any[] = [];

  lregistros: any[] = [];

  constructor(private inventariosService: InventariosService,
    private comprasServices: ComprasService,
    private itemsService: ItemsService,
    public override authService: AuthService, public override logger: LoggerService) {
    super(authService, logger);
  }

  ngOnInit(): void {
    let d = new Date();
    this.d1 = this.d2 = d;
  }

  Buscar() {
    this.itemsService.getItemsPromise().then(resp => {
      this.lregistrositems = resp;
      this.inventariosService.queryInventariosPorFecha(this.fechaToInteger(this.d1), this.fechaToInteger(this.d2))
        .then(resp => {
          this.lregistrosinventarios = resp;
          this.comprasServices.queryComprasPorFecha(this.fechaToInteger(this.d1), this.fechaToInteger(this.d2))
            .then(resp => {
              this.lregistroscompras = resp;
              this.ProcesarReporte();
            });
        });
    });
  }

  ProcesarReporte() {

    this.lregistroscompras.forEach(element => {
      element.items.forEach((element2) => {
        this.ldatacompras.push({ id: element2.id, cantidad: Number(element2.cantidad) });
      });
    });
    const result = Object.values(this.ldatacompras.reduce((r, o) => (r[o.id]
      ? (r[o.id].cantidad += Number(o.cantidad))
      : (r[o.id] = { ...o }), r), {}));
    this.ldatacompras = result;

    this.lregistrosinventarios.forEach(element => {
      element.items.forEach((element2) => {
        this.ldatainventarios.push({ id: element2.id, cantidad: Number(element2.existencia) });
      });
    });
    const result2 = Object.values(this.ldatainventarios.reduce((r, o) => (r[o.id]
      ? (r[o.id].cantidad += Number(o.cantidad))
      : (r[o.id] = { ...o }), r), {}));
    this.ldatainventarios = result2;

    this.lregistrositems.forEach((ele) => {
      let detalle: any = {};
      detalle = ele;
      detalle.sumatoriocompras = this.ldatacompras.find(x=> x.id == ele.id)?.cantidad || 0;
      detalle.sumatorioinventario = this.ldatainventarios.find(x=> x.id == ele.id)?.cantidad || 0;
      detalle.diferencia = detalle.sumatoriocompras - detalle.sumatorioinventario;
      this.lregistros.push(detalle);
    });

  }

  applyFilterGlobal($event: any, stringVal: any, dt: any) {
    dt.filterGlobal(($event.target as HTMLInputElement).value, 'contains');
  }
}
