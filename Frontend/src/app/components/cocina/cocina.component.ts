import { Component } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import Ordencocina from 'src/app/interfaces/ordencocina.interface';
import Produccioncocina from 'src/app/interfaces/produccioncocina.interface';
import { OrdenescocinaService } from 'src/app/services/ordenescocina.service';
import { ProduccioncocinaService } from 'src/app/services/produccioncocina.service';
import { LoggerService } from 'src/app/services/logger.service';


@Component({
  selector: 'app-cocina',
  templateUrl: './cocina.component.html',
  styleUrls: ['./cocina.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class CocinaComponent {

  lordenescocina: Ordencocina[] = [];
  clonedOrdenescocina: { [s: string]: Ordencocina } = {};

  lproduccioncocina: Produccioncocina[] = [];
  clonedProduccioncocina: { [s: string]: Produccioncocina } = {};


  constructor(private ordenescocinaService: OrdenescocinaService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private produccioncocinaService: ProduccioncocinaService,
    private readonly logger: LoggerService) {
  }

  ngOnInit(): void {
    this.getOrdenesCocinaObserver();
    this.getProduccionCocinaObserver();
  }

  getOrdenesCocinaObserver(): void {
    this.ordenescocinaService.getOrdenescocinaObservable().subscribe(productos => {
      this.lordenescocina = productos.filter(x => !x.procesado || !x.recibido || !x.entregado);
    })
  }

  getProduccionCocinaObserver(): void {
    this.produccioncocinaService.getProduccioncocinaObservable().subscribe(productos => {
      this.lproduccioncocina = productos;
    })
  }

  onRowEditInitOC(ordencocina: Ordencocina) {
    this.clonedOrdenescocina[ordencocina.id as string] = { ...ordencocina };
  }

  onRowEditSaveOC(ordencocina: Ordencocina) {
    this.updateOrdenCocina(ordencocina);
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Producto actualizado' });
  }

  onRowEditCancelOC(ordencocina: Ordencocina, index: number) {
    this.lordenescocina[index] = this.clonedOrdenescocina[ordencocina.id as string];
    delete this.clonedOrdenescocina[ordencocina.id as string];
    this.messageService.add({ severity: 'warn', summary: 'Ops!!', detail: 'Producto no fue actualizado!' });
  }

  async updateOrdenCocina(ordencocina: Ordencocina) {
    const response = await this.ordenescocinaService.updateOrdenescocina(ordencocina);
    this.logger.log(response);
  }

  onRowEditInitPC(produccioncocina: Produccioncocina) {
    this.clonedProduccioncocina[produccioncocina.id as string] = { ...produccioncocina };
  }

  onRowEditSavePC(produccioncocina: Produccioncocina) {
    this.updateProduccionCocina(produccioncocina);
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Producto actualizado' });
  }

  onRowEditCancelPC(produccioncocina: Produccioncocina, index: number) {
    this.lproduccioncocina[index] = this.clonedProduccioncocina[produccioncocina.id as string];
    delete this.clonedProduccioncocina[produccioncocina.id as string];
    this.messageService.add({ severity: 'warn', summary: 'Ops!!', detail: 'Producto no fue actualizado!' });
  }

  async updateProduccionCocina(produccioncocina: Produccioncocina) {
    this.logger.log(produccioncocina);
    if (produccioncocina.solicitado && produccioncocina.procesado && produccioncocina.entregado) {
      produccioncocina.solicitado = produccioncocina.procesado = produccioncocina.entregado = false;
      produccioncocina.observacion = '';
    }
    this.logger.log(produccioncocina);
    await this.produccioncocinaService.updateProduccioncocina(produccioncocina);

  }

}
