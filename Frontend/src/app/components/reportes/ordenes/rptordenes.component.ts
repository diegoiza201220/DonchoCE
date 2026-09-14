import { Component } from '@angular/core';
import Orden from 'src/app/interfaces/orden.interface';
import { OrdenesService } from 'src/app/services/ordenes.service';
import { BaseComponent } from 'src/app/util/base.component';
import { MessageService, ConfirmationService } from 'primeng/api';
import { AuthService } from 'src/app/services/auth.service';
import { LoggerService } from 'src/app/services/logger.service';

@Component({
  selector: 'app-rpt-ordenes',
  templateUrl: './rptordenes.component.html',
  styleUrls: ['./rptordenes.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class RptOrdenesComponent extends BaseComponent {
  constructor(private ordenesService: OrdenesService, private messageService: MessageService, private confirmationService: ConfirmationService,
    public override authService: AuthService, public override logger: LoggerService
  ) {
    super(authService, logger);
  }

  [x: string]: any;
  d1 = new Date();
  d2 = new Date();
  lregistros!: Orden[];
  lregistrosorden: any[] = [];
  selectedOrden!: Orden;
  ordenDialogo: boolean = false;
  mostrarEliminar: boolean = false;

  ngOnInit(): void {
    let d = new Date();
    this.d1 = this.d2 = d;
    this.mostrarEliminar = this.emailsPermitidos.indexOf(this.authService.userEmail) !== -1;
  }

  Buscar() {
    let rqOrdenesPorFechas = {
      FechaIni: this.fechaToInteger(this.d1),
      FechaFin: this.fechaToInteger(this.d2),
      SucursalId: this.authService.getLocalStorageDataByKey('sucursalId')
    }
    this.ordenesService.queryOrdenesPorFecha(rqOrdenesPorFechas).then(resp => {
      this.lregistros = resp;
    });
  }

  hideDialog() {
    this.ordenDialogo = false;
  }

  onRowSelect(event: any) {
    this.lregistrosorden = [];
    this.ordenesService.getOrdenDetalle(event.data.id).then(resp => {
      resp.forEach((item: any) => {
        item.preciototal = this.redondear(item.producto.valorDoncho * item.cantidad, 2);
      });
      this.lregistrosorden = resp;
      this.ordenDialogo = true;
    });
  }

  onRowUnselect(event: any) {
  }

  applyFilterGlobal($event: any, stringVal: any, dt: any) {
    dt.filterGlobal(($event.target as HTMLInputElement).value, 'contains');
  }

  deleteOrden() {
    this.confirmationService.confirm({
      message: '¿Estás seguro de eliminar la orden seleccionada?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.ordenesService.deleteOrden(this.selectedOrden.id).then(resp => {
          this.Buscar();
          this.hideDialog();
          this.messageService.add({ severity: 'success', summary: '¡Muy bien!', detail: 'La orden ha sido eliminada', life: 3000 });
        });
      }
    });
  }

  imprimirOrden() {
    const printContents = document.getElementById('ordenDetalle')?.innerHTML;

  }

}
