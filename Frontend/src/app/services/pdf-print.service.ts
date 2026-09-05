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
        images: {
          logoEmpresa: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAPEAAACTCAYAAABf9/9YAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAEe3SURBVHhe7Z17XBTX3f8/u1wW2AsgKguIlwAKCQbMBdMH1CbaKBiN99TUW2yTmCLapL8GxZr2SSqG9GnTikTb9NGgRus1iXcTTB4F20gagZAEFUgURRYQ2SuwLOz+/pid2ZnZmd3ZCwhm36/XvnTPXJiZnc/5fs/3nPM9IovFYoEPHz4GLSKfiH348JyK8jKcOrIPTY0NSEgcj4SkZGTOe5a9W5/wgxNxbU01SktOQtXYgKbGBqpcr9Wg7vLXjH1dRSpXICFpPABgQloGZAriu0weSpX7uPc4dWQv8tevZhdDGR2LzHnPImveYihjRrI3e40fhIj1Wg1KS07gQPF2j4XqCRlTszBhYjomT5vZpz+qj/6jtqYaK+dMYRfbkTl3MTLnLcaEtAz2Jo+550VcW1ON/HXZd1W8XMQnJmPStJmYNC3LZ6UHMVs25eHgru3sYl7iE5OxaPkqr7ra97SIa2uqkbN0Fgw6LXvTgCI1LR0rc3L7pJb20bfkLJ2FyvILjLKMlyei8csmfH/e1lxjI5UrkDX3WSxascpjr+yeFbGqsQErnp484AVMJzUtHWvy8n2WeRDBJeJn9swBABhud+Da+QZcOV0PU4eJsQ+djKlZWJmT6/bvLmYX3CvsKCwYVAIGgMryC1g5Zwp2FBawN/kYhEiHhuCBeYmY9/eZmPjiQwgbFcreBQBQdvYkVs6ZgtKSE+xNgrgnLXFFeRnWLJ3NLh5UxCcmo3D3McgU3D+8j4HBjEdGM4zF8KSheHwDf7NI3aDB1dP1nK62VK7A6f9cYxc75Z4U8Y7CAuzcOvitmTI6Fjs/Ou+ykGtrqlFXU42mxhuAtWlB704DgKiYkVRbLComFsoRIwdcm5x9HxXlZQCAhMTxkClCIVMoMCEtw2031BtMGjeE8d2ZiElMHSZCzKUNMLR2UOVbdh91+Xe4J0XM1U4ZrAixyGTfd2nJCY+j8MroWEyaNhOZ8xY7FYeqsQEVF8sokXlCQlIylDEjoWpsQGnJSVRcLIPqlrDzunLN3kSv1SDz0TGMMqEiprN/yYfU/30itsJ2cYSQOkrCLnKLyutGdpHHxCcmY+dH5xlltTXVOHVkH0pLTgh+2V2FHKywaPkqRiVSUV6GHYUFA7KijE9MxpoN+S4LwRVKS05gy6Y8zuc+ZvJIpL3wELuYl9aa2/h0E+FhAMCOD8+5XBHdkyJmuzhcxCsDkDt7COIiA9ibvEKVVcyV142oumb0WNwLl63Cmg35d0VA9O6Qk0f2DYqmSsbULKzdkO9x9w2b/HXZOPXBPnYxRfK8RDwwL5FdzEvFnmpcPV0PWCvNg59VsXdxyg9WxKmjJPjzsmHs4j7DYDSj0irmM1Ud0HeZ2bs4JT4x2WN32RP8AwLQY+LvKhloSOUKFO4+5rJl4+NA8TYU5m9gFzNwVcTHX/6YahOTFbWr9JuIS0tOoLaGeAHJAEVtTbWd2xufmAyZItQasEhGQtJ4l2vTgShiOgajGYcu6nH4ot4tMftwjbzNW70yQorrvZIOC2EEplwRsbpBgzN5n1Hf84t2Y9K0mYx9hNBnIibHK58vOYmysyfZm11CGR2LhStWCR5zzPWw2dxNEZMYjGYUfNSOsiud7E0+vIynQuYaIx3zcBQmvki0f1tqbqPxyybETR2DiLhwxn58XD1Tj4rd1YAH3UvoCxGrGhtw4L3tOPnBXjsr6w2EDFEcLCImeetoO05XGdjFPryMu5YODsYejJk8EjEPRyHm4Sj2Jqec2fAZ1Nc11PeMqVlYtGKVw3ebC6+JWK/V4EDx9n4LejgaojjYRAyfkPsFqVyBQ59WOeyuc8TCx1M4I9IAEBASgBGPRGHC03EIiHR+fsPtDhz/1cfsYgDAc6tzsTInl13Mi1eGXZaWnMBzT0/uNwGDNkRxy6Y89qZBSfb0UCjD/NnFPryIQafF+uwl7GLBOBKWqcOEpx8LwIqUDswZ1oKH5VqE+vWwd6PQ3dKziyh2bi1AbQ3hZgvBY0vsLOROkpo6GhNSRyE+XgmZLAipKaPYuwAA6uqbUVenQmXlNVRUXodKpWbvYkd8YjLy3iyirHJ/WOJmTS8iQ/3YxR5Rdd2Il3e1sotdRhnmj9RREqSMkkAZ5od4ZQCkEmZ9Xd9sgr7LDH2XGXXNJjSre1F2pXNABdpkQWJkjAtG+rggxCsDGc+76roRZVc6ceFKF1RqfrFwwdc+Li05gYqLF1B7uRpRMSPt5v86mhW34fWxmPhf9m3hdlMAajtC0GAMgtHM/A1MHSaqLd1Sc5sRIHPFGrstYr1Wg5ylsxx2eSiVYcickYLMGamIFOBicNHcrMHBQ5/j1Okq6PVd7M0UUrkCa/PykTnv2T4XcX2zCc//vZlRFq8MQPq4YCyYKLMTjCu461YTL3swUkdLPKpcLlzpxOGLepf7tZdPUWD5ZAW7mOovh7Vy4bq2ZwtVdkKckSJF9vRQQc+y+LwWxefshcUHuz+2orwM+bnZnK5yTt4mLFr+ksP3/VevxuGJJ4eyi+1o7g7ENwYZmroCAZGIvRmfbSpDS81twEUR+/3+97//PbvQGbU11XhuzmQ0c9w0AMTHK/G71+YjJ3s6UlNHQyYLYu8iGJksCGlp8Zjz9CMIDPSHSqXhFLOp24jSsycRFROLUgHRcGWYP6anSNnFgvjDkTtQaXoZZXf0ZlRdN+KzbzqROlqCITL7l1UI8cpAHL7I72qxmZEixRvPDMWsh6WIVwZAFuT8pXfEyKEBmJ4iRepoCQxdFjS0CbNyqaMlnKPelGH+1Ifv2lSaHtQ0dlPfc2cPwfIpCgT627/oXKSOkiAjMRiffdOJ7h7nNkmv0yIqJhZRMSPxP7/7NQrzN0DPYV0BoLz0U0yYmI7fv/wLjwQMADK/XowJ7oSyow273r4Cv0A/KKLlgNUq/2enrWKZNC0LD6Q+SjuaH5ctsSOXQiYLwnMrpmDB/InsTV7DYDCicOtpnDrt+sgWOp5Y4ifeuMkuYqAM88e7LwwXZEW4eGVXq1NLqAzzR+7scKTQheMfCJE8AgAgClNSxRb9HaBTR/zrIheudKLgaLtTN5vPEgvhwpVObDzQBlgFPD0lhL2LIOqbTXh5V6vTawWAYZFRCA2P4BSmUBYvG4HFy2LYxYL4xZJKtKiMVEAsICSAGrkFAAc/rRTUnQpXA1t6rQb567I5BRwfr8SBf67tUwEDgFQqwbrcp7HpD894ZOH7EpW6B6erbO0bV3HmIcxIkeLdF4YTAg6SQTxmAsRpc+A3eQnEEzIhnpAJUewDQJAMFv0dWHRtbgkYANLHBWPfGiXmT5SxN3mN9HHBgPW+3BUwAMRFBiD7yTB2MSetzU2cAvYL9kfYj4bBL9hxkHHq9GEMARu7TGhr1sHYJWxE2/gUosIzdZjw/fkGhoBT09IFCxiuiNhRmyBzRgq2/GU5pFJ7d4oXPwVEIfdDJH8MoiGzIB66gPqI5I9BJBnBPoJBRvo47PjHi4iPt1mc/kJIFPmMG+1akukpIbyu54wUKV6dHQ6pNAjiMRPg91+LIBozASKZNQ7QpYe5phS95/fAXFMKS+t1oEu4e86FVCJG9pNhyJ3tPNbgLqmjJB4JmGR6SginWy+EiCeiELM8DrrKdvR28jcjpk4fhrW/uY/6buruxfXa22hr0eN67W1BQiZFzMWi5avYRQ7hflM42FFYwCnghQsmYl3u08IE7KeASP4YxMN+BnHkSojCniQEGxQHBI6gPiL5YxBFLCD2kT/GPgtFZGQotvxleb8LWckRnGFTpzKhvtn5j8kH1wtNChj+gRA/lAXRmAm2jT3dMNdeRO+/DsDSVEs/zGtMTwlB9nRhls5Vlk9RMJsGHrB8Cr9A+IhZHg956hA0bL/iUMDjUxRY+5v7oG0nRtmZe80wmXoRMzoc4REhCA0PgdHB8STDlYHsIsA6ftrVASmCRFxRXsaZ0S9zRgpWZ09nF9shkoyAKHQKRKHEsDVLVz0sbYdsH93nsOg+B7pvAmZaW5Amevhx/zBSqeSuCFkIdSr3RRwfyfyR45UBhIABiBMm2iwvrNb30klYbnxjK+sj5qfJkGF1f72JtwQM67mEeEskMcvjEfbYMKgOOB72eF9cCDa8Phaqm2rq02Hoxs3v2tDSqAVEImjaO6hyR9wXx2wyZc5djIOfVro1AcKpiPVaDfJzs9nFSE0djXW5T7OLObEYb8KiOQfLnWOUYC3Gm7aPtcx8+xDMqm0wN++ApeNb2wkChgkS8kBrI6s0zmtkPuKVzCmSZFtPJBsCUVQCVW7R30Fv+Ydut3ndIffpcF53f6Ag1KUO+9EwhD02DOrPW9F1k78JdF9cCPL/fD869R2UFda2d0Kv7cLY8VEYkzgcw6IUGDNuOMaMG44QKbelJQmRMr05JS3Tiqs4/SUOFG+36z+TyYKQ/4dnGGVepVcLi/pjmFXbCAttNgJiCcRDZrH3pJBKJdjUl9fkBvUeWGL6POf5E2U2SxVOG6Pb0w3zpZNAj+Nan6S+2YTi81qP3HxY28h9GejyBpFhzps8ABCeHgkAMLXx9wZIZX5Y+2ocQqR+iIh6AJJgmyeibe+E6qZtQFJAoB8CAoX9bUftYldwKGJiPPQ2djHWrxPYBvYUs5Gw0G2HAFMrEDCMcsm5SE0ZhUkZwqaB9QdCujqEsGAi0ZcIqyUmMddeFCxgg9GMl3e1oviclurO8YQZTiLodxuhltjSS/SwioP5hferV+MwJo6IUciHrkbyf32IoBDb6Cy2kIUipY0lqCgvg16rQUV5GSrKy6Bi5URzhEMRc81EmpSRiIz0cYyyPsfUSglZJJ3A61bDWsHcK8iCxIhXBjBHOVn7gdGldymAVXa5i6pUVOoenPGgCwwAIkP9+qRtfLcI+9Fwzm6l2fOV1HBKv4D74C8ZD39JJEY++D4MXbaKzB0hj6G1i2trqrHgiRSsWToba5bOxsInUgXPC3Ao4oPv2QezclY7D2T1CWajTcgOItb94iH0E/GRxFBOOqIgwo11RcCwDqhw9N0d4ljt9sGMX7AfIqYypxOOT1HgFy/ZxviLxLYmxPrc3+NXG5rQfNsmfG17J7TtnTB2mdBh6Eb7bQPamnVQ3VTjxndtuF57G1erm3C1ugkAcF+8rQfCoNPaGcyDu7YLEjKviLkSsE3KSHR7DLRXMBthVn9MdEmJ7x2x8qE3mqEMZVoHMoBlUasY5c5gjwDzhqvv7eBWfbMJrx1ow1tH22Ewen59rjLkiSgEjSCso1Tmhw2vj2Vs7zXVw2Imgl8ln3yCjk4Lfrv5Dm42ES65IjwYivBgtDXrcfO7NrQ2adHWooe2vROdhm67/mO6O83HwV3bnbrWvL9CaYn9+OMFC/p2NJYgTK2wGCogCnQ8GOReoE5lgpInQGNpJ2pzobBFW+dhcAtWT8GbbDzQhrIrnThdZfBoxJu7+AX7YfQrD0A6VoENr4+1iyBbzAZ0qP8GAEibSGiho9OC//4fHc5fDIFyBNGDIAmyd8vZOOqC8gv2Z7j2zlYE4RVxxUVbGk1YZyTxTR/sbyyGCsDJiK6BQJ+4my6KF9agFhu2qN3B25aYPpPpwmXP3X0h9OiYYvIL9sPolx/AtahQtHPUc92GT6C//TpWrJgPAEhMHIMnn/wRfvosEeUGgP/7VzhuqiSobwjG55UKfF6pwNiH8jD2oTykTvkbxo6PctoFFfYj27j+Ux/sc2iNOX+F2ppqDle6n4NZjrBGrQc6nrzkzaxZUiTu9AfzDTrxtKvJm+l+6VMW+xP1v7nnb19Qi/DWNTH+flOML7UihqBNnf/Go8k78EVpIna/K8GG/3cHshDbzDpt53Dkvt6C195UofDvN1D49xtQ3UlGuHIRAoOTbSfiobezB0GxzOg/26jS4XzLyGyUdFJTR7OL7i70kV0DFKHdHFyw59eSUG60v+OaXAjesMZ9BbsN31fov1FD/Tm3kAHg+07gUDMh6PW1hKgPNotwQS1CE88lTvoR07rL5XIk3X8/AKDH+BVjGx+mNiPVPgeA8xzNWxJOEeu19rOU+r1biYVFZ0TPpUbqY9HxPMEBhCtD/9jwtll7umG53UBNObyb8HkL3sCTCtBVVAeuoeObdnYxJ993Ape0IhxvFWFLAyHs463Mec/xY25j6ZLJAICYmBjs3f9PAIC5pwWGO39i7OsIRaptTACXYSXhFDH7gLs1nNGiM8L4zypon94FzbR/QP/Sh9RHM+0f0D69C8Z/VvW7oIWIky+LhVActQktTbUAfey0E/jGJXvi7sOBtzDY6O3swTRzJ54fYUEU96NiEOYP3C+1YOoQC8L8Cde7ywx8qxehpE2EL7UirHmxBcc/eBxnTr2EMSMq0KH+GzRNy6nothBCxtrGQ7C7n+hw/op6rS2NJqxzhfubrnfLoZ2zC90nLiPohTTIts2FbNtchF3MRtjFbEj/mAX/R2LQ9Y8voJ2zC6Zz37FP0WcIGdKXPs79is9gNFPuJJdbaWm9TvUXe4KnbVpeb2GQMT5FgVnzlLgv2II1I834XZwZS6MIkZKfpVHEts0JZuSOMWNptAUPyCyIlliwINKCIDFQpibc7EPNIhR8L0advwrt7XvQqdkDo862aJoQ6IEtZ3CKmD3lUN6PltiiM8Lwm5Mw7v8KIb+bBvnuZ1ARJcOa/V/imG40Nec4cPaLCHn9pwgt+QUki1NhePUUuo9fZp/uriALEjOGSrpK5TWbcKto/6djuc0freSC7Z6yJ1i4w71giYOCxVj7qm1uMAAEiYH7ZRZMiyA+M6LH48GI8XZWOkoCLI224GEF0U/8sIKw5OlhFqh7gOOthJhL2ghLLZSY5fEIGCLAJbDCKWI2MpnwE3qCuUkH/UsfoqfiFmTb5qAuSoYl2YewdPVhlJR+hz9s2oqbLSDmHIfcT8xLHroAQS9kIOS1qeh44+yAEPL8iTKPXGl6ji0uSww3+olTRrNEzJrq6A4XrtjnOhtsTHtyGIZH8r/fIrEUsqGvQT68AIHSn7A3M3hYYcELI8x4apgFD1mF3WUGzt6xiZmr24pEOlaB0a88gLDHCCts7hBWSQoScZOK6V73BaQF7q29Ddm2ObgKC5ZkH0Z5RSO1j07fiSMH3mcch8AREA9ficA5kygh301mpEjdzjUFa1cLW7iedgWBY8KCOxPn6VRdN94TlvhHkx3HFvwC4iASW0dxDXkF/pIH2btwsjDSJmTQxPzWNTEKvici3F8FBGH4U7EY/lQsRr/yAEa//ACkCbR28FVbOzg+kb9rSpCIhWLRGdF9/DIMvzkJ9cQixkcz9V3oX/oAxn9WwdykYx+KzrfL0Ft7G0HPp8EQJceS7MPQ6e2t0Of/OscuIqYpRixA4KwU+D/kXuIyV2APhQSVuG4INXHfXQqO2kdJi864NrCei8hQP+TOHgJZkBjLpyg88hQAeDyBYrAQEMQUrXx4ASTyOYwyPthCJlH3EBHuav8gDJs5AsNmjmCIFwB6O3sZfdhcK52QcIpYKne9ljad+w7aObvQ8cZZ9FTcgv9DMfB/KAaSn6Yg6Pk0+D88Aj2XbqHz7TJiv9fPUmI2/rMK3ScINzjwqUQU/uNzTgE7RCyBSDoBQc+nsbd4nekpIXj3hUi8vWwYPt04Ap9uHIG9OUrOlDqucLhcz2ndKq8bvTIYYnpKCI7+JtojTwHWwJs7ubEHG+23ue8xJOxFKJRFCAl7EQHBP4K/5EFeC80nZGc0Ftcx0gQtdJB3i1PEjlTPRcfrZ2F49RQgEiH4lQwoPlwG8x8zcePVpxCy7qcIzp4L6dvLEHr2eYS8NhV+Y4ei+8RlaOfsQuefy9D5NtGl5Td2KMRKOUrO2zL/sWlsbGEXUYikE+D/UDS7uE+Iiwzg7bpxh6rrRocW15Xk6H3Nxv2ez0ce6HQYuqHXdqGnm/td9Au4DxL5HKq9LB9egNCoYvZugFXITw0jItjO6O3sRcP2K9BV2UbmpaalO9SkgNPC4VIqHa+fRfeJy/AbOxSKD5fBODMR+f/4Nx55cjuWvLgDjZ3WpHfDfga/uFWQLFoM+fvLEPgUMXnfuN+WPzrwqUTU1LaiUWXvbgtCLHE413igUt9scjpRv/K6EYfLPcta6Q3eOtpu12a/FzH3mtHT3Yte03eC+3Z7TdyCB4iI9ZqRZjyk4BezruoO6jdVMQQslSuwJs9x3i3O0yUkMlXPJ+Lu45cpAcu2zcUH5+rw+LydKD5QCVgDUVv+RLsAPwVEoVMgjliAkN9lUUImsei6oXUycOMnTzqOEIr8B5eI65tNKDh6R9AQyKIz6rvaFnV3iZnBiNhPDJOpF9o712HUO+/jtZgNTkdjhQcQVjl3jBnPj7Ag+poarSduomH7FVx+5Qs0bL9ilyZo8zt7HFph8IlYyNKP5iYdOt8uhUgugfSPWTh76QbWbfrEri37ySf/x/gOWBPfWYXsN1bYEhgk859Zyi4atFy40omXd7XyTlDgouDoHbzFEfzqSwxGM1470PaDETAAhEgDIQkKgOqmGurmnQ4Ha1jMBuhaXhVssYPEwH3BFpgrb6Pl+A3oqu7YpcmVyhV4bnUuVDcbsKOwAKUlJxjb6fCI2N6aVVZdZ3zverccFn03AmcmQi8LRO4fPmFsJ9HpeYYPBgyDSDoBwS9PYm/hJe3RB6mB5HxYBsHECFgXAdt4oE2QBWZzusqAF95t9kqwyxlV1414/u8tKPNCJpDBRux9QyBTBOHGd21Qff8X6Fpy0W0ogbmHiMv0GKth1H0ITdNy9JpcHzHYouL+/eITkzF52kzs3FqA/PWrsXNrAfKyl2LGI6M5lzzlFLEz821u0jGiySXn6u0sMJ2Ln3NPGxRJJ8D/kTGUNTY3aZE0dhjkHINL5LJgvPV2EbuYSa8WlnZbv/JA5ExVB54tVHkcqKpTEesOvXW0vU8mIjRrevHagTa8vKuVM2L+Q0DsJ4ZyRBji71fCP9AP2jv/geHOn6BpWo72G5nQtbyKDvXfBFtgNs3N3JppamzgXC7YoNMiZ+ksu2HRnCLmorLyGiw6I7reLYduCTErQxwlh1/CUIfRZIeIJRAF34+AKcSwN9O57yGHCNMmM4fBpT36IM7963OMGOE4EYClq17wGOr+HPdLpop9tlCFgqN3vCqK01UGLN7ShLeOtntsmZs1vThcrscL7zZj8ZamH6T15SNEGuh0Ir+r8FliR5MdDDqtnWvNuyoie43fVyYm48dftMGit82V9H8oBrJtczD2v/7K2JfNpa+qoOBw0QHAovscnW8XouvdcgBA8CsZMM5MxOPz3kNSUgLW/joPEx/jT4xHYTbC3LID+hf3Y2FNNTpEnLfF4N0XIgVNAuAThyxIzDieXLi7zvrv3Vi4m1xgPE4ZgHgn3WDkfdU1m3CmyuBS25yNMsxf0PI2fOiNZsbflwWJPUr/wz4fH5v+lOS1/M+u8H19B9a+aHONzQFimP394N9pf81ho0JhCRRBU0sEmDOmZmHzO3uo7bwiXvh4CiO7x8sRUfjxDeZkZyEinvbEY9i+w941IGGLWCSXQLZtDvzuT4I4YoGwhHjWTJims+Uw/OYkNsgN+Lp38I/r9TayIDFkQWKvegKDnaMldydv3NdfaZH3Sg31XTM6HOYAMcJr7bsaZ/xxKu40qFFe+CVVVnrF1g3F604rRzCXlLhzh9/v52rDkvxkhvM80PQINTmG2tLeSKSo7Xa8FrCl41uYW99H77c16Hj9LESyQMSkDYxcYAMNfZfZJ2AajiY+9DXVlcyxEJrRYeiW21/P2BlxCI2SY3gcMwkE3aXmFfGEtAzG9+5u/h8/KYG7myjt0Qcxf9FP2cVMTK3o+ZIZjDI36Yg5wmfLYb5tXXTNUEEI2vqxdHxLrNmk/hjGvaXQv/QhYLFAtm0ushbdndq1P0gdJXHpM3+ijFgAfIrCbhLED53kFPeni3qKwWDTU3tCBHqCA9AbwGyOhI0KRfI8YiyFdGgIwkbZun5ra2zThXnd6VNH9iJ//Wrq+2JTCH5qss/4H3r2eWzdfwmF/3uRUZ44bgz2HvyQty0MWN1g1TZo5+zinBQBq5UOmHIfAqaMgZ+1sjCrdDDf0qG3thXGfcSECv+HohHy2jSIo4gfZtFP/4rm5r6ffcWGnLcrCxLbZbusVxFtZZWm122L+OlGx8E9R1RdN+LlXfz5pH5o/GNPKoYr7a1ff7Dh1zWortJCF6NA64NE0o2gO52Ivkg0YQNCAjA9/3FIh9rG45f//RK+P0/MI6e3i3lFXFFehjVLZ1PfZ/cE4efd9jW5LRC1Ezq9ETHRQ7HmV792boFBpJ7t+c9h6Jbsp8r8EoYiYMoYmM59j97a24z9ufB/KJqYYMGavbTzvXN4r5hjxpMXUYb5I31cEGH1RksglfA6NnY0a3pRp+rGhStdLgW/fCL2DrPmKfH8L+9es+upzC/QljQcuhibkaOL+IkNGRiWxPRwvzlyGV8fIbp2U9PSUbj7GOBIxGBFqKUWEfZ22s+9FMklUHy4DOX1BtzSJQkSL2BbX0n/4n70XGqESBaIkNemUt1NANBzqZGytHRB+yUMhd/YoQh8KtFOvHR+/ou/oa6+mV3sMaSbyl5ixV0MRjPKLneh+LzWqYX2idhzYkcG4a0tyYJWYHAHc6+ZmgEVLJPYdU199a0BKw7o7NrA0mY9Ii/dQvK8RDxgdaPpuCXinKWzUFl+gfr+Wm8YHjba3zgZpRaF3A+RYorziLJVwMZ95ej8cxn8EoYSx3M07D1BpVJj5S/+BoOBu4vIVeKVAch+Msxht40nGIxmvHdOy8jswcYnYs+Ijg7ES6uHIy4xAjKF8LRTX2pF+FYPdJpF+L6TGDoZJQGiJRakh1kQTms5sRdXkymCED3KNs/8nX934Z1/2ffBh9e1IVkRiIyXuWM6fCJ26P+xg1sXLPZ/GFaLaXj1FMwtlTC3vs9cIJwOue5w6/uUgANnJvaJgGFdtWLLX5YjPs6Wnd9dlk9R4O/PR/aZgGFd9zf7yTC8vWyYx5kofTAJkfph9tyhWL8xFsHBxLNtbdKi7lsVbl1vh6nbftRbl5nIk/Xf9WIcahbhWwMhYHLb953MJPPXdT3oMHRDppAwKgi9touxbMsXN+z7ggEgMkyCiS8+xC52ikNLzG4XA8Dh3uHwN9rfMKyudfDLGQiYch9EskCIaEutWMxGIhJ96Ra63i1Hz6VGBP3i0X6ZxK/Xd2Hne+dw6nSly1ZZFiRG7uxwr7nOQuGzmj5L7DoSiRgv5cQgMSkYwdJAhMiIyQ1tzTq0tRBej9hPhPvGDYfYjxD4l9bc0gJDFRT39+ox0a8T8feFo7OjG91dvfDzF0MRbnt/kv9kP4ElKNAPL0wbBWk4v3fAZ4kdihgcI7c2jhqDR2qIYWEiWSD8xg6FRddtF4QKmHIfo/+358tGmJu0MDfpIFbKEfK7qQ7bs32BXt+Fl365Aw03nAfMSISO6uoLDpfr7RIF+ETsPs8sjsCjaURwNmK4DBGRcui1XWi/bUBPdy9G3BeB2xY/HGsVUxbXHYaKerAg0gzjNeJZBwT4ITI2DCHSQPznZg9W7Gf2xAQH+2PJT0ZhqBNvlE/ETn221LR0xvd3je1U7ufQs89Dtm0u5HuegWzbHEieSaG6gUznvkPXu+XUhwheSRCycSrke57pdwHDKmJXBJw7e8hdEzAAzE+TUV1WPjxn/742fFFOBJzaWvRobdJCpghC7H0RGJM4HGc0/tjS4JmAAeC2xR/vtQRCG0pEnk2mXty6ToywOltnc6vFgWKERodgSeYYpwIGgJYa27sbFWMbjOXUEu8oLMDOrcylFf/3Hy86bGeam3QwNzEHcfslDOVv9/opAIuxz9dXKtx6BocOM/uz+Vg+ReFxLipv0KzpxeIttvS0fWWJ45UBiI8MZCTGr7pmn3nT25BjpOOUAZxxAH2XGfXWMdDeuhbSIgdLAxF7XwS6zMDfb4p511byhDlDejDW0omAQD/IFEHI+IcOnf5+CAgPRIg0AE/HRWBosDBDcfzlj2FoJZJC5ORtwqLlLwFCRMzVLl64YCJWZ09nlLmDSDKCiGYHDIOl41tY1B+zd/EqWU8VCGoTK8P88e4Lw13q9+1LXtnVSr3A3hQxmfkyY1wwb/bLZk0vTlcZPJ46SSd1lAQpo4kRZa4GCps1xKSSwxe5kwoKISTED+tfG4P7HwzDjV5/7L7letvXFRZEEgnmT90C/lBNyG1ocAAyRw+BPJD7ubMx3O7A8V/Z9LFl91Eq8OxUxHqtBgueSGFMj1Iqw7B/3xrGfhR+CojDnwQAWEytRKTaxKz9RUFxQMj9xL8k3Tdhvn2IvptXqay8hrUv72IXc/LGooh+D2Q54kxVBwqOEu6Yt0Q8f6IMK6YoBFdUzZpebDxwW9DMIC6UYf5YPlmBjMQgwX/TGVXXjSg+p3XLQo+JC8HTr92PTzqYVlDTZYGa5k5ruuzl4ScGJkWKkBQG3DISCeGd1SdrRpqx/ksLanVAjEyCzNHhCLQG0YRAbw+DNQHCqYgBIH9dtt0kZT6XWiQZAVHEAnYxIWSLEfAfxt2P3MciFupKK8P8sTfH87Wn6ptNOF1lQL3KhJTREo9cc4PRjFlv3QK8IOKNB9rw9rJhbrX1DUazy+mESGs/P83ztaP4uHClE0Ufa1y2zEMej0LgzFFQd1lg6AbUnU6lQLEyDvh5vG01xCYj8F0nsZgal1seIAIuXO/FaEUInogNY292iKnDhGO/+himDuK5Z85djLw3bQkyBFUFk6ZlsYtw+jSRDI+NpYfH7QoYBgSO4Bawo+O8RGXlNXYRJ54shEZypqoDz/+9GYcv6lFptRae5MWSSogphJ4iCxK7LWBYr+PtZcMErQoJazv73RcimQIOkkEUlQDxmAkQJ02C+KEs2ydhIkThUfRTCCJ9XDDefWE45k90raK481kTaq52olFjcUnAXJBrMJELsk0dwsxqabIAabEBLgsYACr2VFMCBoDMeYsZ2wW9GZOmzbRLKF9adoXxnaJXa+c+C8LJlENPETr8MsNDN7pZ04uij+2zg5Zd6YTB6H7Dy5MJ8iRxkQFuC5hEKhELSpKfOkqCt5cNo9raotgHIE6bA7//WgRx0iSIxkyAKCoB6NTBcuMbmC+dhLn2ostrTJG4O1Am5F/upXOqcFAnB4mBaRFEVssFkRbIyTpPZEaLkX80HhdXz9RTkx5g7S1iD8ISfLdZc59lfFep1LzC4B2xxUev1vVjXKCuTsUu4kQWJHY50MKGbzKDvosYH+0uQq1ff7BgosyhUFJHSfDnZcMglYghkg0hhJswESLamsqWG9+g9/wemGtKYWllJmH0hBRr5eHo+uhIvr4NsYbD//UCQWJAJJJhRNBwDA8k+qdbug3oMjt3+00dJpS9fREVu23ZP6RyBTbQ3GgSYXfKYcLhyKU2VMDSJTzvlkXTt7ONmnjyZrPx1ArDyeLgdc3MzCiuIGRN5P7CkTWOVwbgjWeICewi2RCIH8oCaGspW9Qq9P7rAMy1F4Ee95+HI+IiA5DrwppYQV8LHzsglBtdgdjfPAw1Bhn8RCIMl8gQL41AkNgfTV3c025N5l6oTZ2oOHUZR144gcYvmV7J2rx8KGn9wySCRZyQNB7K6FhG2anTVbxdNhb1x85dZLMRljvHXBK8O9TVcXsMbLwhFJWDzJMXrnS55VLXN5vQrOY/792Ab/3l3NlDqOiz6P7JgL9tBo+lqRbmSyeBLtdcSndIHxcsOAmCxA0Ry3gcI1V3AA42R+C8OgI9FuZOQWJ/jAkZAjMsaOk2oKXbgIZONeoMbfha14wrhtv4/lYrrr5vvzxv3uatyJzH9IZJBEWnSbgGfuSsno4F8zlmXXReA4JHQxQUB1HoFObyKr1aWIw3iWwd7rSfXWTDb/ej7AJPG56GN7qWnnjDScXlIZ5Ep73Ns4UqRkR4/kQZsp8kAjeiYaMgHj+V2mZpqoW5ppT67oxmTS9v37VQDEYzFm9RcTZv2GiWJ6NnOLd3wQU7On3H5If/a5ej02x7fwy93TCZe9FtMcPQ0w2TpRfdZseVceOuOsZqiKQLPWnaTMZ+dARbYgDI4nCpDx7i6bbp0QOqI7DoK2Fu3gHzrb/ArNoGc+v7VFod6L4ixN7H6PXC2qKetjv5smLeq9CDbbIgMVbQ1jxmtH/VKsECNhjNeLZQhcVbmnDBw5S5jtx+Nv433Osd6TIDJ1qDsU8lw3edPfi+ox01+hZ8rWvG9x3tuNmlRYtRD0Nvt1MBd900MAScMTULhz6tcihguCpiZcxIZM5lClmlUuP0GduiaBTSscQwylv/BJqPA5ovgY5rgPoC0P5v4NY+wNQGBI9mH+l12KtX8OFp5PaHBj39UMa4YOYgDlpXkfnb87ZyJxyijcQq+tjz9EpCXWr/FtfWuJIFiFDSJkLB92KUqY242aVDW3cHDL3d6BXu3DJQHbQZNNICC1lSySURg2ed1J3vcQSmxIHA0CeJf41NgOYS0HIcaDtHWN8hPwbk/Kuf++gbDEYzMTvqY7Vb7XM69MkZ7P510hJb1CqX2sD0BeNU6h7Ue5jkPy6Se0w2Gz8XRVzdKcLZO94brtn2aRMMV23ewKLlLwkSMNwRcULSeLuZTbzWODACiF4MhP8ICH2I+Az7CVEW5HqnvrtIpZ51G91LbNzfhqIzahy+qPfqOsPxSu7VESxNtewiXqquG+1GXbkzpJKNkD52Vy2xUWRrD3tKb2cvWo/bYinK6FiszMll7OMIl0UMgPMPFG49wx2pFgcSFjf0YeLTD+4zm4R4z4dR3gvUN5sYoqi8bvTaOk7sIJRFT4ztdXfwBomQoJQzPI11cOHvlnK4ufbnbxirIuYV2PcFO8KtS5mQloGMqcyhmET2DI5lTH0MGLgEUXmNo+J1EYdznl1wpVUc3WjkNERPENp1KNQahwZ5zwo37qpD103bwgwZU7PsRmQ5wy0RA8DaDfarlx88dFFwEGkg4i2rNFDhErFK43z0kDP0XG1rNyww17VwXbOrKEOFWWKR0f7vczFU6h0Rqz9vZUSjldGxnCOynOG2iJUxI7FwmX2Qa/ObH3G71YMAdnvsXqOvVoLUc0zXs6iFDXV1hjeuWSnQEgslwgsiVn/eisbiOuq7VK5A/jt7BAez6LgtYljbxuyJESqVesC51fEC28TeCKI4glyx0J1PX7TrvAVX5WdpbyKGVdJGbLmDNywxl5vuLtJAEYI8/CnYAgaADW8WOV0XnA+PRCxThDKWWCQ5eOgid7T6LiFUxJ6+MPGsZVvYvLEoAn9eNsytjytjgQcKltbrEMmZC4G5isP2tkC43HQuTLHO53wr5Z5ZYS4B5+RtcjqgwxEeiRjWIBefW803y6m/iVIKc1E8HXEllYh5hRyv9GwaoKcVTF/D1edsUdVBFCasAoWXBMuFN4JjJJ640o276uwEnDl3MZUry108FjGsbnV8ov3AjbW/Kh4QQk5NFdatVacyeRzc4pvKOF3gyCE+vNE27Eu4otyudi9xDcpgL0rnDkIqQFMs94QOOpFy91xp0x0j6jd9xQhiAcBzq3MZGTrcxf6puYFMEYq8N4vs2sd6fdeAETJXKiEuuF5GV1gxRcHZfvV0mqM3rUlfwBdPcCXAxTW6iutZuoLBaOa9NjoWifO/Myrcdbm0nriJqxsuMbqRYJ2VxDXewh1cvyoeEpLGc7aPB4qQhbaLvTHo/o1FEdTLR6wgMcRuMISrDHRLzNcUcdUa0ys7WZAYMwROYOBDaCKGXiczmOIixC5ZYfXnrbj620toOU6sckgilSuwZfdR3mmF7uA1EcPaPs7bvJVdTAn5bga7MjLGsYs4KbvS6bFLHRcZgL05Sny6cQSO/iZa8EwaPpo9WM+4v/BGUwTWfN+kNZ6eEuJxZszD5dwT8Nn0hnI3g2Ad3BET6rwtbLpjRNunTbj620toLK6DqY1ZsZGrNrg6mMMZnj0hDjLnPcsr5M1vfoTfbjxwV/qRJ2UkCh5D7c0cy95goF0PHwUf2dKouktkqB/efSESbyyKoOYmu0t9s0lQZk6LxA/dCdzRf2mgCA8o+WXS29kL9eetqN/0Fa5uuATVwWt24pXKFcjJ24TC3cfc7kZyBP/VeQCfkAGgtOwyFv30r4Im6XubSRn2a75ycbrK4BWr4g0MRjPKPHTx+4vK60Zet9oVIkP9PE7OAMBuHSs+uhPCYZHYN3f8xcADSjFjnLShVgv1561QHbyGa29/g8uvlKOxmDl0kk7m3MU49GmVxxFoR/SJiGEVcn7RbrtgF6xWecNv9/e7VfZzIVm30Begr3nvnFZQdHWgwJXp825QfF54Unljsm3hPxKL2oiQ4m+gKvzGKtYv8M1L/8a1P3+DxuI6u6mDbDLnLsaW3UeRJ3BOsCcIf6vdYNK0mSjcfcwuNxdJadllrPzF3/p8vHVdfTPWvrwLJ05WsDfxUnalE4fLhQ/e7wsuWJcrGUzUqUwe5dj2BmeqOgQ3QcwKid0gD3FtOyJ2fY0AlQGGq1oYrmoZs4z4kMoVeG51Lg5+Wom8N4u83vblo09FDGvUeudH5zkHhMA6THPtr4qx9uVdXhezwWDEe8Xn8PNf/E1w8ng6RWfUHker3aW+2YSCuywGdzldZcBrB9o4B4D0NcXntdSSN0Kgt4V727og+agO4R/WQsSzBjcbZXQsFi5bhfyi3Tj9n2tYmZPLmZGyL3EpUZ6nVJSXIT83G6pbzLA7ndTU0cickeJSIIpNXX0zDh36HKVlVwTn1+LD01UT3KG+2YSXd7VyutGeJMorPq+1s1CerP7IXqCNTbwyANlPhvEOgPEmBqMZG/e3CXahYQ1otb+Yim61EcZrOgz/sgnBBv40uqlp6ZDJQ5GQNB4JSclEBth+FiwX/SpigFigbUdhAQ7u2s7eZEfmjBQolWFITR2NhHglr6jr6ptRV6dCZeU1VFReh0pgnmmhyIKI1QU87SoSAn3xNC4Gk4hJZqRIkT091OPuIj7OVHWg6GM1Z6XnCHXiUNwJDoDFZEZ4iwFDWphNl4XLVmHST7IQFTNyQIiVj34XMYkQq8yHUkl0Pbgj1tS0dKzJy0feL5e4/LczxgUj9+nwPnkZha7wNxhFDGtFOH+iDDNSpB4PfIEXljg1KCRQjSTeI39TL0ZdseWelsoV2PzOnn5r03rKXRMxrFb5QPF2HCjexlg6tS+QyhVYm5dPjZSpranGyjlT2Ls5RRYkRsa4YKSPC/JKN8iFK524cKULp6u4uyjYvL1sGLvIISp1LzWLh2vR8BkpUrc9jLpmk1tRfOL5BSN1tMQlQZPCPVNlENT/y0dPgB9uxEfA7EcM4BjapENomy2rB33t38HAXRUxiV6rQWnJCewoLHDZOjpDGR2LhStWIWvus3ahfq5k+K6gDPNH+rggZIwLhjLMX9ALSQ5AqLpu5F236YeELEiM+MgA4vlxTN6vso5lr2s2eeVZmf1EuDVmCIy0MZTR37dTbeGFy1ZhDUfWmoHMgBAxndKSEygtOYmKi2VuC1oZHYsJEzMwaVqW03maXGsvewrflDq2FfTRv3AJGCwRP7c612sTE/qLASdiOqrGBlRcLENT4w3U1lRDryOSidfWVEMuD4VyxEgqWggAEybaooeukLN0FirLL7CLfdxD8AkYAMbUtEDcS8jAJ+JBil6rwaZ12Sg7e5K9ycc9QKc0EKqRYVQbmA47qDXY2sPoj8EegwEyzRB7iRofg5/24VLcGhPOKWBYg1p0EhJd8+IGAj4R08h7swg5eZs4x3v7GFx0hQTgZnwE7gy3rY3MZmiTDlKtLU6ROXexXfBzMOBzpzlQNTZg/S+XoO7y1+xNPgQSGjYEGjX/oJW+ht4PzMbf1IvhN7WM0VlSuQLvfXR+QA/q4MMnYgecOrKX6vbyZJCFj/6Dvj50ywgFdGG2vvxgQzfCWwycQysdLeI90PGJWACnjuzFtI432cU+BiDsRd7JaLSki3tUl5BFvAc6vjaxAAZrDe2DEC+fgJXRsSjcfWxQCxg+Efu41+BKncxGKlcgb/NWHPysyuUxBQMRnzstENOeB9lFPgYgxtmlnLPkpHIFJk+biQlp6Zg0beagjELz4ROxQNwVsUrdA5WmF/HWnMqnqwxoVvciMswPMzxMKE/OSEoZTazX1F9UXjdSY5qnp4R4lBvam+cCgIAlX1H/rygvA4ABP5XQU3wiFoirIq68bsTG/czsFq/ODseZqg5UXTciZZTE5RlJbMggzvLJCiyf0n9928XntCg+T1Qgf142zKMKxJvnAkvEPxR8IhaIqyKe9dYtGIxmSCViyIJEVPbMlFESVF03MmY85T49BPGRASg+p6XyeqWMkmD+RBkyxgWj7Eonis9pUd9sglQiRkZiELKfDMPsP94CrHmuVepeyIJEeGPRUCjD/FBwtJ1KLZQySoLsJ8MY60QVnVFTCelTR0moyRm5s8MZ1rDojBqnqzpgMJoRGeqH7OlhqFeZKOGR9zM9JQS5s4eg7Eonis6o0azpRWSoHzLGBSN7ehj0XWYUHG1H5TUjDEYzdU0XrnQyRKzvMtsdT89F7QyfiH3w4oqIK68b8Yp1svz8NBmmp0gpQXGldY2zTsUj95FKxJQF3zgvAm8caWPsDwDp44I5839NTwlB5TUjVWmQ55JKxDj2ajS13+GLervMlJGhfti3Jor6XnRGzZkscHpKCM5U2ebfkvz6qXD86bh9XrDpKSFQqXvt7lsqIVZ4IP+Go+NzZw9hF3PyQxSxsOrNh9vIgghLnDJagpTRNlcxLtK2SmJ9s4khyAUTbUMF6ckC5qfJCNd5soLhdqaPC6ayjXzf0kMJOC4ygFoGhb0mUfq4IOr/JAsmMhcVo+e7fn1RBPW36dATCnx9w3Z++n6V15gV16uzw7F8soJxn3BwvNClWH6o+ETcByhprnLZlU5sPNCGV3a14pVdreg1E44PIW7b46cn4qOvuzQiwuba1jUTbmzxeS1D3PGRAZSrHBxoG+hPZPWwZW2kX5cyzN8u+R9b2PTrI9uuxee1uKO3tfOVobbrCw607U+vMJRh/oyURvRzdffYHMEhMtv10Z+BkiNZgA8bPhH3Acowf8xPI6xMfbMJ9dYXcnpKCPzE3LNp6IEpelt20WMyqv1Mt2aOAlmkFTMYzdS5uCK/dGtOuvR06H+DvIe4yADE82T+fDQuiKoYyGuVSogkg3SrS48P0IWbHCuhjqc3LRzdqw9fm1gwrrSJSU5XGaglSVNGS5AxLpjRxQQAzWrihV4+RQGVuodqa8YpAxgrBJLHyYLESB8XBGWYP6OLSaXuYXRd0btu0scF2y1+rlL3oOhjDSWWV2eHc3Z50a8pMowINNU1m6hzp4yW2HUR0e8xdZSEqhzqVLZmA3l/XF1MfMcL4YfYJvaJWCDuiHggQ+/akUrE2LdGKTgCPJD5IYp48P9qPtwiMswPKaMkSB8XjDeeibgnBPxDxWeJBXKvWeJ7FZ8l9uHDx6DDJ2IfPgY5Pnfah49Bjs8S+/AxyPGJ2IePQY5PxD58DHL6XMSqxgZqcrZeq8FzT0/GpHFDkLN0Fmprqtm727GjsAALH08RtK+PHxanjuyFXkss7fNDpk9EfOrIXkqsC59IxZqlszFp3BBkPjqGyuVcWX4BqsYG9qGcqG7dQM7SWexiHz9wTn6wDwueSBH8Ht2reFXEpKXNX7/aYeJ1qVyB/KLdgrIMkhbYoNP6rLEPBnqtBgadFiePeHdVy8GGV0W8JT+PId7UtHQ8tzoX+UW78dxq20pzruT5JV1xWHMl+fABq4AdGYofEl4V8fmSE4A1n+/BTytRuPsYVubkYtK0mZgwMR2wphQVKuBTR/bCoCMG6ccnJg+YDIWlJScYlYsPz6goL3P5eZ78YC/1f5licE9V1Gs1KC054XazwKsiJi2lTqexc30npGUgPjGZc9nI2ppqzhs4SVv8e0JaBu8PrWpsYPw9VWMDZjwyGls25TH2Y18T+zusFcepI3sZ28iHvKOwADsKC5CXvRRrls7GgeJtjGPZ6LUau/uqKC/DgeJtjIDMjsICu/1I+O6Zjjv3q2pscPk66Ptu2ZSH/HXZjDK+Y2trqu0CUBXlZdhRWIADxduwZulsrFk6G/nrshn7sKE/z1M0F3oyzSjwvUt8ONuX63ynjuzFwsdTcOqIrSLh+q3Jcq7fUNXYgFNH9uJA8Tasz16CvOylWPhEKuc5nGLxIk03r1tWL3nKkjE23JIxNtwy/eFRlk25v7ScPPy+penmdYtOo7ac/+Q4tf/Vb79i7L96yVOMc5Hl9M+K2ZMY+9CPX/DjBy1NN68zjj15+H3L+U+OW6Y/PMqSMTbc8r9b3rToNGrquBWzJ1l0GrVFp1FbVsyexPhbf/3Des7yDNa1stFp1JZ1L/2Mc9/9771jdx/kfpcullJlf/3Deqp8+sOjGM+NDf1+z39yXND9XrpYSp2bhLzPk4ffp8r2v/cOdS7yfBaLxe585LHrXvoZdezJw+8zjv3rH9YzjqV/yPNwwfU86f+3WCyWSxdLLQt+/KBdOR9Xv/2K8bvS75ncTj/fptxfUttOHn6fKtdp1NRvSj+PTqO2bMr9JVVOvpsW67XSnwv52f/eO9TfcIU+GXZJWq6K8guouFgG1a0bgHXpyDV5+ZApQlFbU42cpbMod5mk9Aqxkl7+umycslpiqVyBhKTxqCy/AADY8eE5yBWhWPH0ZOr4vM1bkb9+NfKLdqO25mvs3FoAWNvl5HEkGVOzGAuK5+RtgurmDSrheHxiMjLnLcaOwgKszMlFYf4Gqpxsh5364ntO916v1SBn6Sy79hq5eDX9vkqv3IGqsQELn0gFrNe1+Z09jH0ypmYBAPQ6DQp3H6Od0caOwgKX71ev1VLHkNc2aRyRjC41LR2Fu49hy6Y86pmkpqUjKmYkamuqsfOj81j4eAr1u9KfC6z3derIXuSvX01tn5CWgYO7tiO/aDfyspfaHce3uDff8yTZsvso9FoNdU5ldCwy5z2LnVsLqHeJDd+7d/DTSiit98i1nTxfztJZ1DNmP+/UtHRsLtpjd81SuQJr8/KROe9ZxvFSuQIGnRbxicnY+dF5an9X8Jo7rddqsKOwADlLZyHz0TE4ULwdK3NycfCzKuQX7YZUrsCpD/ZhRyHx4uSvy6YeUubcxcjbvJUKfqkaGxgCJtbLIV5mWF/o9b9cQh2fk7cJFeUXIJUrMDbpQYabW1l+AalpRHucRK4IhTI6llFGXzFg8zt7EBUzEglJ46HX2n7IzHmLqWVCuAQMAOuzbUuixicmU/c1IS0DqsYGRtwAtDgCrM2RA8XbqHuPT0zG5GmEALN4FkDXazUu3++IUfcxjpmQloFS2nXI5KEoLTlBPRNldCxW5uTifMkJTJo2E6eO7KUEDAAJSeOpNZ2lcgVqa6opAUvlCqzMycXJD/Yic+5iNNHcxQlpGdS18i3uTQ+WKqNjsXDZKmqbMjoWUTEjscnqhkvlCuQVFKG05ITdMyDRazXIo707GVOzkJO3CQCg02rstpPvJrmPqrGBIdrammqqooX1N9xRWMB4B7bsPopFy1/ChIlEJaW6aXsGG94sAqzPwl28IuJTR/Yi89Ex2Lm1gLrBustfU6F/+gXWXq5m3GRqWjpW5uTir/l52LmVaJORQgeAtXn5SEgaj9ISmyW5VneF8cOeOrIP50tOoHD3MZygBcNgtdD0h5YxNQsT0tKplzA1LR0VF20/SnxiMpQxI6lrP01rlx98bzsKdx9DftFuqozOqSN7GTVs3ptFlJWsrSHum7y2lTm5VMVHMnPBzxjfJ6RlIH/9amTOXcy7qNuB4u0O7zdz7mK7+71cXUkdQ1ac9Pb0ohWrGN/jk8ZjzdLZmDxtJlbm5DKukRQLeb6suc9iS77t2MnTZmLTumxExYzEmrx8xrtw8oO92PBmEfI2b6XK6NTWVDMr8z3HUHvZ1q4nr4X82wlJ47H+l0sAAJuL9lD70Tn5ga0CIj0fsn2t1xG/B337wuWrkL9+NQrzN9i9m1K5Apvf2cNo8z6ROYeq/MjtWzYR7zZZYZNiBoDamq9x8NNKKvDrDh6LeMumPOSvXw1ldCzyNm9F6ZU7KL1yBxlTs5CQlIyK8jI8R3N7ExLHMwICa/LyGVbVoNcxfrjMec+ioryM4b6Ul31GHQ/rj/feR+cpS0by3OpcKEeMpH4UZXQsNliFRbIyJ5dyNaVyBeouf42Fj6eg7OxJXP32KwyNtOVhVt26gYryMt7oOj0QtzYvH1vy86i/rddpqPsiXb4t+XmMGv/qN1XUd6lcgabGBmzZfRR51tqaDdsK5+RtsrvfNXn5dvdLPn+pXIFFy1cxXtzUtHTI5KEMS6vXaZC3eSvy3ixiWGGpXAGZPJS6r9S0dMxc8DOGpWpqbMDavHzs/Og8ZIpQRkDKoNPifMkJZM57ltOzoYtjbV4+mmhWkHyG5N+G9XksWv4SCncf4zwfAMoYSOUKrN2Qj/x12ZRBUN1soKLepAeRZ60USE+D/vcKdx9DxcULjN+Q/m6SlQx5frmcaEZWXLTd14HibVDGjOR9p4TgsYhPfrCX8ufp1mLytCyUlpzEmqWzGT965rzFjDYy2/V4/92/0s5B3Bj7JSRFlzl3MQ5+VoW8N4ugjBmJP/33/2O4ScQLa3voeQVFaGpsYLywX335ObV9bV4+tuw+CtWtG8iYmgVl1Ah8V1vD6OOureFum8HqysJ6HxXlFxgvnL+fLdnbmg35RBSc9kKsycvHDmsbVSpX4NCnVdj8zh5MSMuAXks0H9iRy5Mf2LyOjKlZWLT8Jaf3q9dqqO8rc3JRWnKCahvDeh30KDHZVZg571notRoUvfUatW3zO3uw+Z092PHhOez48BwKdx+DVmNL/p5ftJs6FgCWZv0IRw8UM54nvbnChr4tnmZlYb324m3/Q33P27wVOz86j5U5uZApQrFlUx7DWJCQv8mEtAxUXCxj/AY6HTF4hNyevy6belYT0jKw739tHsNzq3ORkGQzSFK5Amvy8hkGSE6r4AAgZuQY/Hze45ApQikX3KDT2v2uruKRiFWNDTDotGiyuhlbNuUhZ+kszHhkNPLXr2bcgNTasKcvJXnqg32M2jZz3mJ8fr6E+i6TEz8G+eAJ15DoqiK3g2aRzn18nDo2ax7RhiTbYGRwpY7WzZKQOB4fvL+D+q4cMRKnjuyDVK7ArIVL8V1tDTa8WcToh6S3zfloorXpAWDStJnYnLeG+l5b8zXVZoRVgFvy89DW2gJY21WkJSGDLHJFqN2iYPSXXOj90t1k1c0b+CvN9Y1PTEZpyUncuFZPlZHPmLyObiORmVIZHUu5xglJ4zmXCCW3qxobsHHNClyrv4IFS19guI5C3ciVc6ZQXZhSuQLxSeMZFb5yBLFN1diA/HXZvB4T+e6UnT3J+A1S09LxxYX/o76XnT3JaL9PnpaFT44for4vWk60zUmRZ819FqUlJ9DZQaxmYdBpGcYnc+5i5K9fDYvZzAhQKqNj7X5XV/E4Ok2PtHEhtS4pSUalYXUhCvM3ID4xGWs25GPN0tnU/vGJyWhqbIBcHsqoGcmon0wRSrUz6SijY/HK7/8H//3r52HQaalIIvm3Fi5bhTUb8qFqbGBEtRcuW0W5uqTLRD7klXOmUOcnKyG+timsTQv2kpokGVOz0Ntjwr/PfQJldCzy3yEimOR1xCcm44VXNlLXT0K4vC9hZY7NepHQ78WV+5UpQrFzawGk1qAh3aVURsfit3/cjtxVixnXQR6rHBHLOD8Xzz092S6a/Ej646ip+g8Meh1V5ugcsFbOC55IgUGnxcJlqzDpJ1mMphcZKGO/f/R3hU1pyQnkZS+lLCW92RCfmIzk1Efx4T93Ij4xmXCnrVFvAJg4aSq+rvwCCUnjqXeEfq9SuQKrc19HwW9/RT1b+m8skyugpz1T8j3gqgBdwWMRwxrQOV9yEnqdrUN/QloGEpL4R2dVlJchIXE8ZIpQqBob0NTYAJk81BoR1hBCtnZFJSSNt6utamuqUVdTDZnVQtEfREV5GSOAotdqoNdpGOeoKC9jLHlZUV6G2ppqTJ42kyqrramm9puQlsH5UrCpKC/Dgfe2Uy7/wmWrsGjFKihjRkKv1aD2cjV1beR38r5hFWaTdfBKQtJ46hk5wt37pZ+7oryMcR3ktXFdB+n+sX8TEvax5N8lo/Nk5J/veDqqxgbotBq762LfC/tvcaG3dn2S7yo9CEkfCkx/nrU11dDrNHbvJvt9g9XTkVnfWdJzIq8XtAFL5LXSfzNP8IqIfTAhvROybetMhD76HtKqk00VtvWGtV3tyNMaqHjUJvbBDdl2m3yPrUg/mGmyxm8SksZT7XxYA1RkNxm9d2Ew4RNxH7AyJxdSuYLXrfPR/yQkjUfG1Cyc+mAfys6epAZhrMzJReHuY8jbvJWqfAcbPne6j+Bql/rw0Rf4ROzDxyDn/wOSppDMKFuWRAAAAABJRU5ErkJggg==' // tu base64 aquí
        },
        content: [
          {
            image: 'logoEmpresa',
            width: 100,        // ancho en puntos (ajusta según tu ticket de 80mm)
            alignment: 'center',
            margin: [0, 0, 0, 8]
          },
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
          { text: 'item.nombre', fontSize: 9, border: [false, false, false, false] },
          { text: item.cantidad.toString(), fontSize: 9, alignment: 'center', border: [false, false, false, false] },
          { text: `$${Number(item.precioUnitario.toFixed(2)) + Number(item.impuestoValorUnitario.toFixed(2))}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
          { text: `$${Number(item.precioTotal.toFixed(2)) + Number(item.impuestoValorTotal.toFixed(2))}`, fontSize: 9, alignment: 'right', border: [false, false, false, false] },
        ];
      });


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
                ...filasItemsFact
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
}
