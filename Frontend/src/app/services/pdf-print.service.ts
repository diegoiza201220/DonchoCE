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
    await pdfMake.createPdf(definicion).download(`orden-${orden.FechaInteger}-${orden.secuencial}.pdf`);
  }

  // ---------------------------------------------------------------------
  // CONSTRUCCIÓN DEL DOCUMENTO
  // ---------------------------------------------------------------------

  private construirDefinicionPdf(orden: any): TDocumentDefinitions {

    const filasItems = orden.facDetalleOrdens.map((item: any) => {
      //const subtotal = item.Cantidad * item.PrecioUnitario;
      return [
        { text: 'item.nombre', fontSize: 9, border: [false, false, false, false] },
        { text: item.cantidad.toString(), fontSize: 9, alignment: 'center', border: [false, false, false, false] },
        { text: `$${item.precioUnitario.toFixed(2)}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
        { text: `$${item.precioTotal.toFixed(2)}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
      ];
    });

    //const total = orden.detalle.reduce((acc: number, i: any) => acc + i.cantidad * i.precioUnitario, 0);

    return {
      pageSize: {
        width: this.ANCHO_PAGINA_PT,
        height: 'auto' as any
      },
      pageMargins: [this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT, this.MARGEN_PT],

      content: [
        { text: 'orden.nombreComercial', style: 'encabezado', alignment: 'center' },
        { text: 'orden.razonSocial', style: 'encabezado', alignment: 'center' },
        { text: `RUC: ${orden.rucDonCho}`, style: 'textoChico', alignment: 'center' },
        { text: `Matriz: ${orden.direccionmatriz}`, style: 'textoChico', alignment: 'center' },
        { text: `CONTRIBUYENTE ESPECIAL: ${orden.contribuyenteEspecial}`, style: 'textoChico', alignment: 'center' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: orden.sucursalNombre, style: 'textoChico', alignment: 'center' },
        { text: '', style: 'textoChico', alignment: 'center' }, //Linea en blanco
        { text: `Fact.Elect: ${orden.establecimiento}-${orden.PuntoEmision}-${orden.NumeroFactura}`, style: 'textoChico' },
        { text: `Clav.Acces: ${orden.claveNumeroAutorizacion}`, style: 'textoChico' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `Nombre: ${orden.clienteNombre}`, style: 'textoChico' },
        { text: `CI/RUC: ${orden.clienteRuc}`, style: 'textoChico' },
        { text: `Fecha Emisión: ${orden.fecha}`, style: 'textoChico' },
        { text: `Orden: ${orden.fechaInteger}--${orden.secuencial}`, style: 'textoChico' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        // { text: `DESCRIPCIÓN                    CANT.     P.UNIT      P.TOTAL`, style: 'textoChico' },
        {
          table: {
            widths: ['*', 20, 35, 40],
            body: [
              [
                { text: 'Desc.', fontSize: 10, bold: true, border: [false, false, false, true] },
                { text: 'Cant', fontSize: 10, bold: true, alignment: 'center', border: [false, false, false, true] },
                { text: 'P.Uni.', fontSize: 10, bold: true, alignment: 'right', border: [false, false, false, true] },
                { text: 'P.Tot.', fontSize: 10, bold: true, alignment: 'right', border: [false, false, false, true] },
              ],
              ...filasItems
            ]
          },
          layout: 'noBorders'
        },


        // { text: `Fecha: ${orden.fecha.toLocaleString('es-EC')}`, style: 'textoChico', margin: [0, 0, 0, 6] },

        // { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: `Subtotal: $${orden.totalSinImpuestos.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: `Base ${orden.impuestoPorcentaje}%: $${orden.impuestoBaseImponible.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: `Impuesto ${orden.impuestoPorcentaje}%: $${orden.impuestoValor.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: `Total: $${orden.totalOrden.toFixed(2)}`, style: 'total', alignment: 'right' },
        { text: '--------------------------------', alignment: 'center', fontSize: 7, margin: [0, 4, 0, 4] },
        { text: '¡Gracias por su compra!', alignment: 'center', fontSize: 7, margin: [0, 6, 0, 0] },

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
