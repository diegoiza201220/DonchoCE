import { Component } from '@angular/core';
import { MessageService, SelectItem, ConfirmationService } from 'primeng/api';
import Item from 'src/app/interfaces/item.interface';
import { AuthService } from 'src/app/services/auth.service';
import { ItemsService } from 'src/app/services/items.service';
import { BaseComponent } from 'src/app/util/base.component';
import { LoggerService } from 'src/app/services/logger.service';  
@Component({
  selector: 'app-items',
  templateUrl: './items.component.html',
  styleUrls: ['./items.component.css'],
  providers: [MessageService, ConfirmationService]
})

export class ItemsComponent extends BaseComponent {

  item!: Item;
  litems: Item[] = [];
  clonedItem: { [s: string]: Item } = {};
  selectedItems!: Item[];
  submitted!: boolean;
  itemDialogo!: boolean;

  lfamilia: SelectItem[] = [
    { label: 'Aliños', value: 'ALINOS' },
    { label: 'Carnes', value: 'CARNES' },
    { label: 'Enlatados', value: 'ENLATADOS' },
    { label: 'Frutas', value: 'FRUTAS' },
    { label: 'Lacteos', value: 'LACTEOS' },
    { label: 'Mariscos', value: 'MARISCOS' },
    { label: 'Vegetales', value: 'VEGETALES' },
    { label: 'Bebidad', value: 'BEBIDAS' }
  ];

  lubicacion: SelectItem[] = [
    { label: 'Alacena', value: 'ALACENA' },
    { label: 'Congelador', value: 'CONGELADOR' },
    { label: 'Barra', value: 'BARRA' },
    { label: 'Cocina', value: 'COCINA' }
  ];

  lunidad: SelectItem[] = [
    { label: 'Unidad', value: 'UNIDAD' },
    { label: 'Libras', value: 'LIBRAS' },
    { label: 'Litros', value: 'LITROS' }
  ];

  constructor(private itemService: ItemsService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public override authService: AuthService,
    public override logger: LoggerService) {
    super(authService, logger);
  }

  ngOnInit(): void {
    this.getItemsObserver();
  }

  openNew() {
    this.item = { nombre: '', familia: '', ubicacion: '', unidad: '' };
    this.submitted = false;
    this.itemDialogo = true;
  }

  hideDialog() {
    this.itemDialogo = false;
    this.submitted = false;
  }

  saveItem() {
    this.submitted = true;
    this.addItem();
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Item creado' });
    this.itemDialogo = false;
  }


  getItemsObserver(): void {
    this.itemService.getItemsObservable().subscribe(items => {
      this.litems = items;
    })
  }

  async addItem() {
    const response = await this.itemService.addItem(this.item);
  }

  async deleteItem(item: Item) {
    const response = await this.itemService.deleteItem(item);
  }

  async updateItem(item: Item) {
    const response = await this.itemService.updateItem(item);
  }

  onRowEditInit(item: Item) {
    this.clonedItem[item.id as string] = { ...item };
  }

  onRowEditSave(item: Item) {
    this.updateItem(item);
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Item actualizado' });
  }

  onRowEditCancel(item: Item, index: number) {
    this.litems[index] = this.clonedItem[item.id as string];
    delete this.clonedItem[item.id as string];
    this.messageService.add({ severity: 'warn', summary: 'Ops!!', detail: 'Item no fue actualizado!' });
  }

  deleteSelectedItems() {
    this.confirmationService.confirm({
      message: '¿Estás seguro de eliminar los registros seleccionados?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.selectedItems.forEach(element => {
          this.deleteItem(element);
        });
        this.messageService.add({ severity: 'success', summary: '¡Muy bien!', detail: 'Items han sido eliminados', life: 3000 });
      }
    });
  }


}
