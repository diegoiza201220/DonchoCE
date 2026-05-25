import { Component, inject } from '@angular/core';
import Compra from '@interfaces/compra.interface';
import { ComprasService } from '@services/compras.service';
import { BaseComponent } from '@util/base.component';
import { AuthService } from '@services/auth.service';
import { LoggerService } from '@services/logger.service';

@Component({
  selector: 'app-rpt-compras',
  templateUrl: './rptcompras.component.html',
  styleUrls: ['./rptcompras.component.css']
})
export class RptComprasComponent extends BaseComponent {
  [x: string]: any;

  d1 = new Date();
  d2 = new Date();
  lregistros!: Compra[];
  total: number = 0;
  lregistroscompra: any[] = [];
  selectedCompra!: Compra;
  compraDialogo: boolean = false;

  private readonly comprasService = inject(ComprasService); 
  public override authService = inject(AuthService);
   public override logger = inject(LoggerService);

  constructor() {
    super();
  }

  ngOnInit(): void {
    let d = new Date();
    this.d1 = this.d2 = d;
  }

  Buscar() {
    this.comprasService.queryComprasPorFecha(this.fechaToInteger(this.d1), this.fechaToInteger(this.d2)).then(resp => {
      this.lregistros = resp;
      this.calculateTotal();
    });

  }

  calculateTotal() {
    this.total = 0;
    if (this.lregistros) {
      for (let registro of this.lregistros) {
        this.total += registro.total;
      }
    }
  }

  hideDialog() {
    this.compraDialogo = false;
  }

  onRowSelect(event: any) {
    this.lregistroscompra = [];
    const lista = this.lregistros;
    const compra = lista.filter(x => x.id == event.data.id);
    this.lregistroscompra = compra[0].items;
    this.compraDialogo = true;
  }

  onRowUnselect(event: any) {
  }

  applyFilterGlobal($event: any, stringVal: any, dt: any) {
    dt.filterGlobal(($event.target as HTMLInputElement).value, 'contains');
  }
}
