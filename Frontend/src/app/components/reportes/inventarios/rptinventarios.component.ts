import { Component } from '@angular/core';
import { BaseComponent } from 'src/app/util/base.component';
import { AuthService } from 'src/app/services/auth.service';
import Inventario from 'src/app/interfaces/inventario.interface';
import { InventariosService } from 'src/app/services/inventarios.service';
import { LoggerService } from 'src/app/services/logger.service';  

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

  constructor(private inventariosService: InventariosService, public override authService: AuthService, public override logger: LoggerService) {
    super(authService, logger);
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
