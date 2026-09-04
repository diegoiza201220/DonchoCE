import { Injectable } from '@angular/core';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import { TDocumentDefinitions } from 'pdfmake/interfaces';

(pdfMake as any).addVirtualFileSystem(pdfFonts);

export interface ItemTicket {
  descripcion: string;
  cantidad: number;
  precioUnitario: number;
}

@Injectable({ providedIn: 'root' })
export class PdfPrintService {

  /** 80mm en puntos PDF (1mm = 2.8346 pt). Se resta un margen de ~3mm por lado. */
  private readonly ANCHO_PAGINA_PT = 226.77;
  private readonly MARGEN_PT = 8;

  /**
   * Genera el PDF del ticket y dispara el diálogo de impresión del navegador.
   *
   * IMPORTANTE (pdfmake 0.3.x): la ventana debe abrirse de forma SÍNCRONA,
   * dentro del mismo gesto de clic del usuario, o el navegador bloqueará
   * el popup. Por eso se abre "en blanco" primero y se le pasa a print().
   */
  async imprimirTicket(datos: {
    empresa: string;
    ruc: string;
    direccion: string;
    numeroComprobante: string;
    fecha: Date;
    items: ItemTicket[];
  }): Promise<void> {
    // 1. Abrir la ventana YA, de forma síncrona (aún vacía)
    const ventana = window.open('', '_blank');

    if (!ventana) {
      // Bloqueado por el navegador: alternativa, forzar descarga
      await this.descargarTicket(datos);
      return;
    }

    const definicion = this.construirDefinicionPdf(datos);

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
  async descargarTicket(datos: Parameters<PdfPrintService['imprimirTicket']>[0]): Promise<void> {
    const definicion = this.construirDefinicionPdf(datos);
    await pdfMake.createPdf(definicion).download(`ticket-${datos.numeroComprobante}.pdf`);
  }

  // ---------------------------------------------------------------------
  // CONSTRUCCIÓN DEL DOCUMENTO
  // ---------------------------------------------------------------------

  private construirDefinicionPdf(datos: {
    empresa: string;
    ruc: string;
    direccion: string;
    numeroComprobante: string;
    fecha: Date;
    items: ItemTicket[];
  }): TDocumentDefinitions {

    const filasItems = datos.items.map(item => {
      const subtotal = item.cantidad * item.precioUnitario;
      return [
        { text: item.descripcion, fontSize: 7, border: [false, false, false, false] },
        { text: item.cantidad.toString(), fontSize: 7, alignment: 'center', border: [false, false, false, false] },
        { text: `$${item.precioUnitario.toFixed(2)}`, fontSize: 7, alignment: 'right', border: [false, false, false, false] },
        { text: `$${subtotal.toFixed(2)}`, fontSize: 7, alignment: 'right', border: [false, false, false, false] },
      ];
    });

    const total = datos.items.reduce((acc, i) => acc + i.cantidad * i.precioUnitario, 0);

    return {
      pageSize: {
        width: this.ANCHO_PAGINA_PT,
        // Alto dinámico: pdfmake soporta 'auto' vía height grande + margen ajustado,
        // aquí usamos un alto generoso ya que la impresora corta según el contenido real.
        height: 'auto' as any
      },
      pageMargins: [this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT],

      content: [
        { text: datos.empresa, style: 'encabezado', alignment: 'center' },
        { text: `RUC: ${datos.ruc}`, style: 'textoChico', alignment: 'center' },
        { text: datos.direccion, style: 'textoChico', alignment: 'center' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },

        { text: `Comprobante: ${datos.numeroComprobante}`, style: 'textoChico' },
        { text: `Fecha: ${datos.fecha.toLocaleString('es-EC')}`, style: 'textoChico', margin: [0, 0, 0, 6] },

        // {
        //   table: {
        //     widths: ['*', 20, 35, 40],
        //     body: [
        //       [
        //         { text: 'Desc.', fontSize: 7, bold: true, border: [false, false, false, true] },
        //         { text: 'Cant', fontSize: 7, bold: true, alignment: 'center', border: [false, false, false, true] },
        //         { text: 'P.U.', fontSize: 7, bold: true, alignment: 'right', border: [false, false, false, true] },
        //         { text: 'Subt.', fontSize: 7, bold: true, alignment: 'right', border: [false, false, false, true] },
        //       ],
        //       ...filasItems
        //     ]
        //   },
        //   layout: 'noBorders'
        // },

        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
                { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `TOTAL: $${total.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },
      ],

      styles: {
        encabezado: { fontSize: 10, bold: true, margin: [0, 0, 0, 2] },
        textoChico: { fontSize: 7 },
        total: { fontSize: 9, bold: true }
      },

      defaultStyle: {
        fontSize: 7
      }
    };
  }
}
