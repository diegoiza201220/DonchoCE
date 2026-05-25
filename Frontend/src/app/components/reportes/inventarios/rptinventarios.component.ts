import { Component, inject } from '@angular/core';
import { BaseComponent } from '@util/base.component';
import { AuthService } from '@services/auth.service';
import Inventario from '@interfaces/inventario.interface';
import { InventariosService } from '@services/inventarios.service';
import { LoggerService } from '@services/logger.service';

@Component({
  selector: 'app-rpt-inventario',
  templateUrl: './rptinventarios.component.html',
  styleUrls: ['./rptinventarios.component.css']
})
export class RptInventariosComponent extends BaseComponent {
  [x: string]: any;

  d1 = new Date();
  d2 = new Date();
  lregistros!: Inventario[];
  lregistrosinventario: any[] = [];
  selectedInventario!: Inventario;
  inventarioDialogo: boolean = false;

  private readonly inventariosService = inject(InventariosService);
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
    this.inventariosService.queryInventariosPorFecha(this.fechaToInteger(this.d1), this.fechaToInteger(this.d2)).then(resp => {
      this.lregistros = resp;
    });

  }


  hideDialog() {
    this.inventarioDialogo = false;
  }

  onRowSelect(event: any) {
    this.lregistrosinventario = [];
    const lista = this.lregistros;
    const inventario = lista.filter(x => x.id == event.data.id);
    this.lregistrosinventario = inventario[0].items;
    this.inventarioDialogo = true;
  }

  onRowUnselect(event: any) {
  }

  applyFilterGlobal($event: any, stringVal: any, dt: any) {
    dt.filterGlobal(($event.target as HTMLInputElement).value, 'contains');
  }
}
