using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using FreelanceApi2;
using FreelanceApi2.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlServer<ReceiptsContext>(builder.Configuration.GetConnectionString("cnReceipts"));

builder.Services.AddCors(options =>
{
    // options.AddDefaultPolicy(policy =>
    // {
    //     policy.WithOrigins("http://127.0.0.1:5173/");
    // });
    options.AddPolicy("NewPolicy", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/dbconexion", ([FromServices] ReceiptsContext dbContext) =>
{
    dbContext.Database.EnsureCreated();
    return Task.FromResult(Results.Ok("Base de datos en memoria: " + dbContext.Database.IsInMemory()));

});

app.MapGet("/api/receipts", ([FromServices] ReceiptsContext dbContext) =>
{
    return Task.FromResult(Results.Ok(dbContext.Receipts));

});


app.MapPost("/api/receipts", async ([FromServices] ReceiptsContext dbContext, [FromBody] Receipt receipt) =>
{
    receipt.ReceiptId = Guid.NewGuid();
    receipt.CreateAt = DateTime.Now;
    await dbContext.AddAsync(receipt);
    //await dbContext.Tareas.AddAsync(receipt);

    await dbContext.SaveChangesAsync();

    return Results.Ok(receipt);
});

app.MapPut("/api/receipts/{id}", async ([FromServices] ReceiptsContext dbContext, [FromBody] Receipt receipt, [FromRoute] Guid id) =>
{
    var currentReceipt = dbContext.Receipts?.Find(id);

    if (currentReceipt != null)
    {
        // currentReceipt.CategoriaId = tarea.CategoriaId;
        currentReceipt.Title = receipt.Title;
        currentReceipt.Description = receipt.Description;
        currentReceipt.Name = receipt.Name;
        currentReceipt.LastName = receipt.LastName;
        currentReceipt.Address = receipt.Address;
        currentReceipt.currency = receipt.currency;
        currentReceipt.payment = receipt.payment;
        currentReceipt.Logo = receipt.Logo;
        currentReceipt.CreateAt = DateTime.Now;

        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }

    return Results.NotFound();
});

app.MapDelete("/api/receipts/{id}", async ([FromServices] ReceiptsContext dbContext, [FromRoute] Guid id) =>
{
    var currentRecipt = dbContext.Receipts?.Find(id);

    if (currentRecipt != null)
    {
        dbContext.Remove(currentRecipt);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }

    return Results.NotFound();
});

app.MapPost("/api/generate-pdf/{id}", ([FromServices] ReceiptsContext dbContext, [FromRoute] Guid id) =>
{

    var receipt = dbContext.Receipts?.Find(id);

    // instantiate the html to pdf converter
    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();

    var otherHtmlCode = $@"<!DOCTYPE html>
    <html>
    <head>
        <style>
            .invoice-box {{
                max-width: 800px;
                margin: auto;
                padding: 30px;
                border: 1px solid #eee;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.15);
                font-size: 16px;
                line-height: 24px;
                font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial,
                    sans-serif;
                color: #555;
            }}

            .invoice-box table {{
                width: 100%;
                line-height: inherit;
                text-align: left;
            }}

            .invoice-box table td {{
                padding: 5px;
                vertical-align: top;
            }}

            .invoice-box table tr td:nth-child(2) {{
                text-align: right;
            }}

            .invoice-box table tr.top table td {{
                padding-bottom: 20px;
            }}

            .invoice-box table tr.top table td.title {{
                font-size: 45px;
                line-height: 45px;
                color: #333;
            }}

            .invoice-box table tr.information table td {{
                padding-bottom: 40px;
            }}

            .invoice-box table tr.heading td {{
                background: #eee;
                border-bottom: 1px solid #ddd;
                font-weight: bold;
            }}

            .invoice-box table tr.details td {{
                padding-bottom: 20px;
            }}

            .invoice-box table tr.item td {{
                border-bottom: 1px solid #eee;
            }}

            .invoice-box table tr.item.last td {{
                border-bottom: none;
            }}

            .invoice-box table tr.total td:nth-child(2) {{
                border-top: 2px solid #eee;
                font-weight: bold;
            }}

            @media only screen and (max-width: 600px) {{
                .invoice-box table tr.top table td {{
                    width: 100%;
                    display: block;
                    text-align: center;
                }}

                .invoice-box table tr.information table td {{
                    width: 100%;
                    display: block;
                    text-align: center;
                }}
            }}

            /** RTL **/
            .invoice-box.rtl {{
                direction: rtl;
                font-family: Tahoma, 'Helvetica Neue', 'Helvetica', Helvetica,
                    Arial, sans-serif;
            }}

            .invoice-box.rtl table {{
                text-align: right;
            }}

            .invoice-box.rtl table tr td:nth-child(2) {{
                text-align: left;
            }}
        </style>
    </head>
    <body>
        <div class='invoice-box'>
            <h1>{receipt?.Title}</h1>
            <h3>{receipt?.Description}</h3>
            <table cellpadding='0' cellspacing='0'>
                <tr class='top'>
                    <td colspan='2'>
                        <table>
                            <tr>
                                <td class='title'>
                                    <img
                                        src={receipt?.Logo}
                                        style='object-fit: contain;'
                                        width='100'
                                        height='100'
                                    />
                                </td>
                                <td>
                                    Recibo #: {receipt?.ReceiptId}<br />
                                    Fecha de creación: {receipt?.CreateAt}<br />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr class='information'>
                    <td colspan='2'>
                        <table>
                            <tr>
                                <td>
                                    Nombres: {receipt?.Name} {receipt?.LastName}<br />
                                    {receipt?.TypeDocument}: {receipt?.NumberDocument}<br />
                                    Dirección: {receipt?.Address}
                                </td>

                                <td>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <tr class='heading'>
                    <td>Item</td>
                    <td>Detalle</td>
                </tr>

                <tr class='item'>
                    <td>Moneda</td>
                    <td>{receipt?.currency}</td>
                </tr>

                <tr class='item last'>
                    <td>Monto a cobrar</td>
                    <td>{receipt?.payment}</td>
                </tr>

                <tr class='total'>
                    <td></td>
                    <td>Total: {receipt?.currency} {receipt?.payment}</td>
                </tr>
            </table>
        </div>
    </body>
    </html>";

    // convert the url to pdf
    SelectPdf.PdfDocument doc = converter.ConvertHtmlString(otherHtmlCode);

    // save pdf document
    byte[] data = doc.Save();

    var result = Convert.ToBase64String(data);
    // close pdf document
    doc.Close();
    return result;
});

app.UseCors("NewPolicy");


app.Run();
