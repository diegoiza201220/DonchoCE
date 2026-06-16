import { Component, OnInit } from '@angular/core';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ClientesService } from 'src/app/services/clientes.service';
import { AuthService } from 'src/app/services/auth.service';
import { LoggerService } from 'src/app/services/logger.service';
import { BaseComponent } from 'src/app/util/base.component';

@Component({
  selector: 'app-clientes',
  templateUrl: './clientes.component.html',
  styleUrls: ['./clientes.component.css']
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
    this.cliente = {
      id: 0,
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
    if (!this.validarCliente(this.cliente)) {
      return;
    }
    this.addCliente();
    this.clienteDialogo = false;
  }

  validarCliente(cliente: any): boolean {
    let mensaje = '';
    if (cliente.nombre.trim() === '') {
      mensaje += 'Nombre es requerido. ';
    }
    if (cliente.apellido.trim() === '') {
      mensaje += 'Apellido es requerido. ';
    }
    if (cliente.cedulaRuc.trim() === '') {
      mensaje += 'Cédula/Ruc es requerido. ';
    }
    if (cliente.email.trim() === '') {
      mensaje += 'Email es requerido. ';
    }
    if (mensaje !== '') {
      this.messageService.add({ severity: 'warn', summary: 'Ops!! ', detail: mensaje });
      return false;
    }
    return true;
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
    this.cliente.fechaCumpleanios = this.cliente.fechaCumpleanios.indexOf('-') !== -1 ?
      this.cliente.fechaCumpleanios.replaceAll('-', '') : this.cliente.fechaCumpleanios;
    this.clientesService.addItem(this.cliente).then(response => {
      this.logger.log(response);
      this.getClientesPromise();
      this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Cliente creado' });
    }).catch(error => {
      this.logger.log(error);
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo crear el cliente' });
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
    if (!this.validarCliente(cliente)) {
      return;
    }
    this.lclientes = [];
    cliente.fechaCumpleanios = cliente.fechaCumpleanios.toString().indexOf('-') !== -1 ?
      cliente.fechaCumpleanios.toString().replaceAll('-', '') : cliente.fechaCumpleanios.toString();
    this.clientesService.updateCliente(cliente)
      .then(response => {
        this.logger.log(response);
        this.getClientesPromise();
            this.messageService.add({ severity: 'success', summary: '¡Muy bien! ', detail: 'Cliente actualizado' });
      }).catch(error => {
        this.logger.log(error);
        this.messageService.add({ severity: 'error', summary: 'Error', detail: 'No se pudo actualizar el cliente' });
      });
  }

  onRowEditInit(cliente: any) {
    this.clonedClientes[cliente.id as string] = { ...cliente };
  }

  onRowEditSave(cliente: any) {
    this.updateCliente(cliente);
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

  handleBlurFechaCumpleanios(event: any) {
    if (!this.tryConvertIntToDate(event.target.value)) {
      event.target.value = "0000-00-00";
      this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Fecha de cumpleaños no es válida' });
    } 
  }

}
