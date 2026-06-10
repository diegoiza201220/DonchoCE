import { Component, OnInit } from '@angular/core';
import { MessageService, SelectItem, ConfirmationService } from 'primeng/api';
import { ClientesService } from 'src/app/services/clientes.service';
import { AuthService } from 'src/app/services/auth.service';
import { LoggerService } from 'src/app/services/logger.service';
import { BaseComponent } from 'src/app/util/base.component';

@Component({
  selector: 'app-clientes',
  templateUrl: './clientes.component.html',
  styleUrls: ['./clientes.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class ClientesComponent extends BaseComponent implements OnInit {
  cliente: any;
  lclientes: any = [];
  clonedClientes: { [s: string]: any } = {};
  selectedClientes!: any[];
  submitted!: boolean;
  clienteDialogo!: boolean;

  constructor(private readonly clientesService: ClientesService,
    private readonly messageService: MessageService,
    private readonly confirmationService: ConfirmationService,
    public override authService: AuthService,
    public override logger: LoggerService
  ) {
    super(authService, logger);
  }

  ngOnInit(): void {
    this.getClientesPromise();
  }

  openNew() {
    let d = new Date();
    this.cliente = { id:0,
      nombre: '', apellido: '', cedulaRuc: '', telefonoCelular: '', email: '',
      fechaCumpleanios: this.fechaToInteger(d), direccion: '', usuarioRegistro: this.authService.userEmail
    };
    this.submitted = false;
    this.clienteDialogo = true;
  }

  hideDialog() {
    this.clienteDialogo = false;
    this.submitted = false;
  }

  saveCliente() {
    this.submitted = true;
    this.addCliente();
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Cliente creado' });
    this.clienteDialogo = false;
  }


  getClientesPromise(): void {
    this.lclientes = [];
    this.clientesService.getClientesPromise().then(data => {
      data.forEach((cliente: any) => {
        this.lclientes.push(cliente);
      });
    })
  }

  async addCliente() {
    this.lclientes = [];
    this.clientesService.addItem(this.cliente).then(response => {
      this.logger.log(response);
      this.getClientesPromise();
    });
  }

  async deleteCliente(cliente: any) {
    this.lclientes = [];
    this.clientesService.deleteCliente(cliente)
    .then(response => {
      this.logger.log(response);
      this.getClientesPromise();
    });     
  }

  async updateCliente(cliente: any) {
    this.lclientes = [];
    this.clientesService.updateCliente(cliente)
      .then(response => {
        this.logger.log(response);
        this.getClientesPromise();
      });
  }

  onRowEditInit(cliente: any) {
    this.clonedClientes[cliente.id as string] = { ...cliente };
  }

  onRowEditSave(cliente: any) {
    this.updateCliente(cliente);
    this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Cliente actualizado' });
  }

  onRowEditCancel(cliente: any, index: number) {
    this.lclientes[index] = this.clonedClientes[cliente.id as string];
    delete this.clonedClientes[cliente.id as string];
    this.messageService.add({ severity: 'warn', summary: 'Ops!!', detail: 'Cliente no fue actualizado!' });
  }

  deleteSelectedClientes() {
    this.confirmationService.confirm({
      message: '¿Estás seguro de eliminar los registros seleccionados?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.selectedClientes.forEach(element => {
          this.deleteCliente(element);
        });
        this.messageService.add({ severity: 'success', summary: '¡Muy bien!', detail: 'Clientes han sido eliminados', life: 3000 });
        this.getClientesPromise();
      }
    });
  }
}
