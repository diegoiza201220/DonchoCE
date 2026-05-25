import { Component, inject } from '@angular/core';
import Orden from '@interfaces/orden.interface';
import { OrdenesService } from '@services/ordenes.service';
//import * as FileSaver from 'file-saver';
import { BaseComponent } from '@util/base.component';
import { AuthService } from '@services/auth.service';
import { LoggerService } from '@services/logger.service';

@Component({
  selector: 'app-rpt-ventas',
  templateUrl: './rptventas.component.html',
  styleUrls: ['./rptventas.component.css']
})
export class RptVentasComponent extends BaseComponent {
  [x: string]: any;

  d1 = new Date();
  d2 = new Date();
  lregistros!: any[];

  private readonly ordenesService = inject(OrdenesService); 
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

    let rqOrdenesPorFechas = {
      FechaIni: this.fechaToInteger(this.d1),
      FechaFin: this.fechaToInteger(this.d2)
    }
    this.ordenesService.queryOrdenesPorFecha(rqOrdenesPorFechas).then(resp => {
      this.lregistros = resp;
    });

  }

  // exportPdf() {
  //   import('jspdf').then((jsPDF) => {
  //     import('jspdf-autotable').then((x) => {
  //       const doc = new jsPDF.default('p', 'px', 'a4');
  //       (doc as any).autoTable(this.exportColumns, this.products);
  //       doc.save('products.pdf');
  //     });
  //   });

  // exportExcel() {
  //   import('xlsx').then((xlsx) => {
  //     const worksheet = xlsx.utils.json_to_sheet(this.lregistros);
  //     const workbook = { Sheets: { data: worksheet }, SheetNames: ['data'] };
  //     const excelBuffer: any = xlsx.write(workbook, { bookType: 'xlsx', type: 'array' });
  //     this.saveAsExcelFile(excelBuffer, 'products');
  //   });
  // }

  // saveAsExcelFile(buffer: any, fileName: string): void {
  //   let EXCEL_TYPE = 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;charset=UTF-8';
  //   let EXCEL_EXTENSION = '.xlsx';
  //   const data: Blob = new Blob([buffer], {
  //     type: EXCEL_TYPE
  //   });
  //   FileSaver.saveAs(data, fileName + '_export_' + new Date().getTime() + EXCEL_EXTENSION);
  // }

  calculateTipodepagoTotal(tipopago: string) {
    let total = 0;

    if (this.lregistros) {
      for (let registro of this.lregistros) {
        if (registro.tipoPago === tipopago) {
          total+= registro.totalOrden;
        }
      }
    }

    return total;
  }
}


