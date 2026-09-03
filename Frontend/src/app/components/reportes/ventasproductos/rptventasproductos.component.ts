import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { OrdenesService } from 'src/app/services/ordenes.service';
import { BaseComponent } from 'src/app/util/base.component';
import { LoggerService } from 'src/app/services/logger.service';

@Component({
  selector: 'app-rpt-ventasproductos',
  templateUrl: './rptventasproductos.component.html',
  styleUrls: ['./rptventasproductos.component.css']
})

export class RptVentasproductosComponent extends BaseComponent implements OnInit {

  d1 = new Date();
  d2 = new Date();
  lregistros!: any[];
  ready = false;


  ldata: any[] = [];
  bgcolor: any[] = [];
  ldataBar: any[] = [];

  basicDataBar: any;
  basicOptionsBar: any;

  basicDataPie: any;
  basicOptionsPie: any;

  constructor(private readonly ordenesService: OrdenesService,
    public override authService: AuthService,
    public override logger: LoggerService) {
    super(authService, logger);
  }

  ngOnInit(): void {
    let d = new Date();
    this.d1 = this.d2 = d;
  }

  Buscar() {
    this.ldata = [];
    this.lregistros = [];
    let rqOrdenesPorFechas = {
      FechaIni: this.fechaToInteger(this.d1),
      FechaFin: this.fechaToInteger(this.d2),
      SucursalId: this.authService.getLocalStorageDataByKey('sucursalId')
    }
    this.ordenesService.queryProductosVendidosPorFecha(rqOrdenesPorFechas).then(resp => {
      this.ldata = resp;
      this.ready = true;
    });
  }

  procesarData() {
    this.procesarDataBar();
    this.procesarDataPie();
    this.ready = false;
  }

  procesarDataPie() {
    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    let ldataPie = this.ldata;

    const label: any[] = [];
    const data: any[] = [];

    ldataPie.forEach(element => {
      label.push(element.plato);
      data.push(element.cantidad);
    });

    this.basicDataPie = {
      labels: label,
      datasets: [
        {
          data: data,
          backgroundColor: this.bgcolor,
        }
      ]
    };

    this.basicOptionsPie = {
      plugins: {
        legend: {
          labels: {
            usePointStyle: true,
            color: textColor
          }
        }
      }
    };

  }

  procesarDataBar() {

    const documentStyle = getComputedStyle(document.documentElement);
    const textColor = documentStyle.getPropertyValue('--text-color');
    const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
    const surfaceBorder = documentStyle.getPropertyValue('--surface-border');

    this.ldataBar = [];

    this.ldataBar = this.ldata;
    const label: any[] = [];
    const data: any[] = [];

    this.ldataBar.forEach(element => {
      label.push(element.plato);
      data.push(element.cantidad);
      this.bgcolor.push(this.randomRGB());
    });

    this.basicDataBar = {
      labels: label,
      datasets: [
        {
          label: 'Ventas',
          data: data,
          backgroundColor: this.bgcolor,
          borderWidth: 1
        }
      ]
    };

    this.basicOptionsBar = {
      plugins: {
        legend: {
          labels: {
            color: textColor
          }
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        },
        x: {
          ticks: {
            color: textColorSecondary
          },
          grid: {
            color: surfaceBorder,
            drawBorder: false
          }
        }
      }
    };
  }

  randomRGB(): string {
    let x = Math.floor(Math.random() * 256);
    let y = Math.floor(Math.random() * 256);
    let z = Math.floor(Math.random() * 256);
    let RGBColor = "rgb(" + x + "," + y + "," + z + ")";
    return RGBColor;
  }
}
