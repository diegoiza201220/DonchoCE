import { Injectable } from '@angular/core';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import { TDocumentDefinitions } from 'pdfmake/interfaces';

(pdfMake as any).addVirtualFileSystem(pdfFonts);

// export interface ItemTicket {
//   descripcion: string;
//   cantidad: number;
//   precioUnitario: number;
// }

@Injectable({ providedIn: 'root' })
export class PdfPrintService {

  /** 80mm en puntos PDF (1mm = 2.8346 pt). Se resta un margen de ~3mm por lado. */
  private readonly ANCHO_PAGINA_PT = 200; //226.77;
  private readonly MARGEN_PT = 2;//8;
  orden: any;
  /**
   * Genera el PDF del ticket y dispara el diálogo de impresión del navegador.
   *
   * IMPORTANTE (pdfmake 0.3.x): la ventana debe abrirse de forma SÍNCRONA,
   * dentro del mismo gesto de clic del usuario, o el navegador bloqueará
   * el popup. Por eso se abre "en blanco" primero y se le pasa a print().
   */
  async imprimirTicket(orden: any): Promise<void> {
    this.orden = orden;
    // 1. Abrir la ventana YA, de forma síncrona (aún vacía)
    const ventana = window.open('', '_blank');
    if (!ventana) {
      // Bloqueado por el navegador: alternativa, forzar descarga
      await this.descargarTicket(orden);
      return;
    }

    const definicion = this.construirDefinicionPdf(orden);

    try {
      // 2. print() en 0.3.x devuelve una Promise y acepta la ventana ya abierta
      await pdfMake.createPdf(definicion).print(ventana);
    } catch (err) {
      console.error('[pdfmake] Error al imprimir', err);
      ventana.close();
      throw err;
    }
  }

  /** Solo genera y descarga el PDF, sin abrir el diálogo de impresión */
  async descargarTicket(orden: Parameters<PdfPrintService['imprimirTicket']>[0]): Promise<void> {
    const definicion = this.construirDefinicionPdf(orden);
    await pdfMake.createPdf(definicion).download(`orden-${orden.fechaInteger}-${orden.secuencial}.pdf`);
  }

  // ---------------------------------------------------------------------
  // CONSTRUCCIÓN DEL DOCUMENTO
  // ---------------------------------------------------------------------

  private construirDefinicionPdf(orden: any): TDocumentDefinitions {

    // Format date and hour (e.g., MM/DD/YYYY, HH:MM:SS)
    const utcDateString = orden.fecha; // Assuming orden.fecha is in UTC format
    const formattedDateTime = new Date(utcDateString);
    const localFormattedDate = formattedDateTime.toLocaleString();



    if (!orden.esFactura) {

      const filasItemsNV = orden.facDetalleOrdens.map((item: any) => {
        return [
          { text: item.nombre, fontSize: 9, border: [false, false, false, false] },
          { text: item.cantidad.toString(), fontSize: 9, alignment: 'center', border: [false, false, false, false] },
          { text: `$${Number(Number(item.precioUnitario.toFixed(2)) + Number(item.impuestoValorUnitario.toFixed(2))).toFixed(2)}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
          { text: `$${Number(Number(item.precioTotal.toFixed(2)) + Number(item.impuestoValorTotal.toFixed(2))).toFixed(2)}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
        ];
      });

      return {
        pageSize: {
          width: this.ANCHO_PAGINA_PT,
          height: 'auto' as any
        },
        pageMargins: [this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT],
        // images: {
        //   logoEmpresa: this.logoempresa
        // },
        content: [
          // {
          //   image: 'logoEmpresa',
          //   width: 100,        // ancho en puntos (ajusta según tu ticket de 80mm)
          //   alignment: 'center',
          //   margin: [0, 0, 0, 8]
          // },
          { text: orden.sucursalNombre, style: 'textoChico', alignment: 'center' },
          { text: '', style: 'textoChico', alignment: 'center' }, //Linea en blanco
          { text: 'PRECUENTA', style: 'textoChico', alignment: 'center' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          { text: `Nombre: ${orden.clienteNombre}`, style: 'textoChico' },
          { text: `CI/RUC: ${orden.clienteRuc}`, style: 'textoChico' },
          { text: `Fecha Emisión: ${localFormattedDate}`, style: 'textoChico' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          { text: `No. Orden: ${orden.fechaInteger} -- ${orden.secuencial}`, style: 'textoChico' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          {
            table: {
              widths: ['*', 20, 35, 40],
              body: [
                [
                  { text: 'Desc.', fontSize: 9, bold: false, border: [false, false, false, true] },
                  { text: 'Cant', fontSize: 9, bold: false, alignment: 'center', border: [false, false, false, true] },
                  { text: 'P.Uni.', fontSize: 9, bold: false, alignment: 'right', border: [false, false, false, true] },
                  { text: 'P.Tot.', fontSize: 9, bold: false, alignment: 'right', border: [false, false, false, true] },
                ],
                ...filasItemsNV
              ]
            },
            layout: 'noBorders'
          },
          { text: `Total: $${orden.totalOrden.toFixed(2)}`, style: 'total', alignment: 'right' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 9, margin: [0, 6, 0, 0] },

        ],

        styles: {
          encabezado: { fontSize: 12, bold: true, margin: [0, 0, 0, 2] },
          textoChico: { fontSize: 10 },
          total: { fontSize: 10, bold: false }
        },

        defaultStyle: {
          fontSize: 10
        }
      };


    } else {

      const filasItemsFact = orden.facDetalleOrdens.map((item: any) => {
        return [
          { text: item.nombre, fontSize: 9, border: [false, false, false, false] },
          { text: item.cantidad.toString(), fontSize: 9, alignment: 'center', border: [false, false, false, false] },
          { text: `$${item.precioUnitario.toFixed(2)}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
          { text: `$${item.precioTotal.toFixed(2)}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
        ];
      });


      return {
        pageSize: {
          width: this.ANCHO_PAGINA_PT,
          height: 'auto' as any
        },
        pageMargins: [this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT],
        // images: {
        //   logoEmpresa: this.logoempresa
        // },
        content: [
          // {
          //   image: 'logoEmpresa',
          //   width: 100,        // ancho en puntos (ajusta según tu ticket de 80mm)
          //   alignment: 'center',
          //   margin: [0, 0, 0, 8]
          // },
          { text: orden.nombreComercial, style: 'textoChico', alignment: 'center' },
          { text: `RUC: ${orden.rucDonCho}`, style: 'textoChico', alignment: 'center' },
          // { text: `Matriz: ${orden.direccionmatriz}`, style: 'textoChico', alignment: 'center' },
          { text: orden.contribuyenteRimpe, style: 'textoChico', alignment: 'center' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          { text: orden.sucursalNombre, style: 'textoChico', alignment: 'center' },
          { text: `Fact.Elect: ${orden.establecimiento}-${orden.puntoEmision}-${orden.numeroFactura}`, style: 'textoChico' },
          { text: `Clav.Acces: ${orden.claveNumeroAutorizacion}`, style: 'textoChico' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          { text: `Nombre: ${orden.clienteNombre}`, style: 'textoChico' },
          { text: `CI/RUC: ${orden.clienteRuc}`, style: 'textoChico' },
          { text: `Dirección: ${orden.clienteDireccion}`, style: 'textoChico' },
          { text: `Fecha Emisión: ${localFormattedDate}`, style: 'textoChico' },
          { text: `Orden: ${orden.fechaInteger} -- ${orden.secuencial}`, style: 'textoChico' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          {
            table: {
              widths: ['*', 20, 35, 40],
              body: [
                [
                  { text: 'Desc.', fontSize: 9, bold: false, border: [false, false, false, true] },
                  { text: 'Cant', fontSize: 9, bold: false, alignment: 'center', border: [false, false, false, true] },
                  { text: 'P.Uni.', fontSize: 9, bold: false, alignment: 'right', border: [false, false, false, true] },
                  { text: 'P.Tot.', fontSize: 9, bold: false, alignment: 'right', border: [false, false, false, true] },
                ],
                ...filasItemsFact
              ]
            },
            layout: 'noBorders'
          },


          { text: `Subtotal: $${orden.totalSinImpuestos.toFixed(2)}`, style: 'total', alignment: 'right' },
          { text: `Base ${orden.impuestoPorcentaje}%: $${orden.impuestoBaseImponible.toFixed(2)}`, style: 'total', alignment: 'right' },
          { text: `Impuesto ${orden.impuestoPorcentaje}%: $${orden.impuestoValor.toFixed(2)}`, style: 'total', alignment: 'right' },
          { text: `Total: $${orden.totalOrden.toFixed(2)}`, style: 'total', alignment: 'right' },
          { text: '-'.repeat(78), alignment: 'center', fontSize: 9, margin: [0, 4, 0, 4] },
          { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 9, margin: [0, 6, 0, 0] },
          { text: 'Su Factura Electrónica', alignment: 'center', fontSize: 9, margin: [0, 6, 0, 0] },
          { text: 'la encontrará en', alignment: 'center', fontSize: 9, margin: [0, 6, 0, 0] },
          { text: 'WWW.SRI.GOB.EC', alignment: 'center', fontSize: 9, margin: [0, 6, 0, 0] },

          // (el resto del contenido repetido se mantiene igual)
        ],

        styles: {
          encabezado: { fontSize: 12, bold: true, margin: [0, 0, 0, 2] },
          textoChico: { fontSize: 10 },
          total: { fontSize: 10, bold: false }
        },

        defaultStyle: {
          fontSize: 10
        }
      };
    }
  }
}
