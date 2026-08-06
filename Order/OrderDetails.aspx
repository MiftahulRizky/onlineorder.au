<%@ Page Title="Order Details" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="OrderDetails.aspx.vb" Inherits="Order_OrderDetails" MaintainScrollPositionOnPostback="true" Debug="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">

                <div class="col-lg-2 col-sm-12 col-md-12">
                    <div class="page-pretitle">Order</div>
                    <h2 class="page-title">Order Details</h2>
                </div>

                <div class="col-lg-10 col-sm-12 col-md-12 text-end">

                    <!-- buton finish -->
                    <button type="button" class="btn my-button btn-cyan" id="btnFinish">
                        <i class="ti ti-checks fs-2 opacity-50 me-2"></i>
                        Finish
                    </button>

                    <!-- button preview -->
                    <button class="btn  btn-secondary dropdown-toggle" data-bs-toggle="dropdown">
                        <i class="ti ti-file-invoice fs-2 opacity-50 me-2"></i>
                        Preview
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <li>
                            <a class="dropdown-item my-button" href="javascript:void(0)" id="btnPreviewPrint">
                                <i class="ti ti-printer fs-2 me-2 opacity-50"></i> Preview / Print
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" href="javascript:void(0)" id="btnPreviewPDF">
                                    <i class="ti ti-file-type-pdf fs-2 me-2 opacity-50"></i> Download PDF
                            </a>
                        </li>
                    </div>

                    <!-- button creat job sheet -->
                    <button class="btn btn-outline-teal dropdown-toggle " type="button" data-bs-toggle="dropdown" aria-expanded="false" id="btnJobSheet">
                        <i class="ti ti-file-text fs-2 opacity-50 me-2"></i> Job Sheet
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li>
                            <a href="javascript:void(0)" id="btnConvertToJob" class="dropdown-item">
                                <i class="ti ti-file-zip fs-2 me-2 opacity-50"></i> Convert To Job
                            </a>
                        </li>
                        <li>
                            <a href="javascript:void(0)" id="btnReprintJobSheet" class="dropdown-item">
                                <i class="ti ti-printer fs-2 me-2 opacity-50"></i> Reprint Job Sheet
                            </a>
                        </li>
                        <li>
                            <a href="javascript:void(0)" id="btnChangeJobStatus" class="dropdown-item" hidden >
                                <i class="bi bi-clipboard-check me-2 opacity-50"></i> Change Job Status
                            </a>
                        </li>
                    </ul>

                    <!-- button submit -->
                    <a href="javascript:void(0)" id="btnSubmit" class="btn  btn-success">
                        <i class="ti ti-send fs-2 opacity-50 me-2" ></i> Submit
                    </a>

                        <!-- button edit -->
                    <button type="button" id="btnEditHeader" class="btn  btn-primary">
                        <i class="ti ti-edit fs-2 opacity-50 me-2" ></i> Edit
                    </button>

                    <!-- button delete -->
                    <a href="javascript:void(0)" id="btnDeleteHeader" class="btn  btn-danger">
                        <i class="ti ti-trash fs-2 opacity-50 me-2" ></i> Delete
                    </a>

                    <!-- button quote -->
                    <button class="btn  btn-dark dropdown-toggle" data-bs-toggle="dropdown" id="btnQuote">
                        <i class="ti ti-file-description fs-2 me-2 opacity-50" ></i> Quote
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a href="javascript:void(0)" id="btnQuoteDetail" class="dropdown-item">
                            <i class="ti ti-info-square-rounded fs-2 me-2 opacity-50"></i> Detail
                        </a>
                        <a href="javascript:void(0)" id="btnDownloadQuote" class="dropdown-item">
                            <i class="ti ti-file-download fs-2 me-2 opacity-50"></i> Download
                        </a>
                    </div>
                    <!-- button administrator -->
                    <!-- <button class="btn  btn-dark dropdown-toggle" data-bs-toggle="dropdown" id="btnAdministrator">Admin</button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a href="javascript:void(0)" id="btnChangeStatus" class="dropdown-item">
                            <i class="bi bi-clipboard-check me-2 opacity-50"></i> Change Status
                        </a>
                        <a href="javascript:void(0)" id="btnSendOrderMail" class="dropdown-item">
                            <i class="bi bi-send-fill me-2 opacity-50"></i> Send Manual Order
                        </a>
                    </div> -->

                    <!-- button refresh pricing -->
                     <button class="btn  btn-purple dropdown-toggle" data-bs-toggle="dropdown" id="btnMoreAction">
                        <i class="ti ti-category fs-2 me-2 opacity-50" ></i> More
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">

                        <a href="javascript:void(0)" id="btnReloadPricing" class="dropdown-item">
                            <i class="ti ti-credit-card me-2 fs-2 opacity-50"></i> Reload Pricing
                        </a>
                        <a href="javascript:void(0)" id="btnChangeStatus" class="dropdown-item">
                            <i class="ti ti-exchange fs-2 me-2 opacity-50"></i> Change Status
                        </a>
                        <a href="javascript:void(0)" id="btnSendOrderMail" class="dropdown-item">
                            <i class="ti ti-mail-forward fs-2 me-2 opacity-50"></i> Send Manual Order
                        </a>
                        <a href="javascript:void(0)" id="btnDownloadBarcode" class="dropdown-item">
                            <i class="ti ti-file-barcode fs-2 me-2 opacity-50"></i> Download Barcode
                        </a>
                        <a href="javascript:void(0)" id="btnQuoteDisc" class="dropdown-item">
                            <i class="ti ti-pencil-discount fs-2 me-2 opacity-50"></i> Override Customer Discount
                        </a>

                        <div class="dropdown-divider" id="dividerPrintQuote"></div>
                        <a href="javascript:void(0)" id="btnPrintQuote" class="dropdown-item">
                            <i class="ti ti-printer fs-2 me-2 opacity-50"></i> Print Quote
                        </a>
                        <a href="javascript:void(0)" id="btnEmailQuote" class="dropdown-item">
                            <i class="ti ti-mail fs-2 me-2 opacity-50"></i> Email Quote
                        </a>

                        <div class="dropdown-divider" id="dividerEmailDeposit"></div>
                        <a href="javascript:void(0)" id="btnEmailDeposit" class="dropdown-item">
                            <i class="ti ti-mail-dollar fs-2 me-2 opacity-50"></i> Email Deposite Request
                        </a>

                        <div class="dropdown-divider" id="dividerLogs"></div>
                        <a href="javascript:void(0)" id="btnLogs" class="dropdown-item">
                            <i class="ti ti-logout fs-2 me-2 opacity-50"></i> Logs
                        </a>
                        
                       
                    </div>

                    <!-- button refresh pricing -->
                    <!-- <button type="button" id="btnReloadPricing" class="btn  btn-outline-indigo">
                        <i class="bi bi-credit-card-2-back me-2"></i> Reload Pricing
                    </button> -->

                </div>

            </div>
        </div>
    </div><!-- /page-header -->

    <div class="page-body">
        <div class="container-xl">

            <!-- card information header 1-->
            <div class="row mb-3">
                <div class="col-lg-7">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-12 col-lg-12 col-md-12 mb-4">
                                    <span style="font-size:larger;">Retailer Name :</span>
                                    <br />
                                    <span  id="spanRetailerName" style="font-size: larger; font-weight: bold;" ></span>
                                    <span  id="spanRetailerId" style="font-size: larger; font-weight: bold;" hidden></span>
                                </div>

                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Order # :</span>
                                    <br />
                                    <span  id="spanOrderId" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-5">
                                    <span style="font-size:larger;">Customer Order Number :</span>
                                    <br />
                                    <span  id="spanOrderNo" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-5">
                                    <span style="font-size:larger;">Customer Order Name :</span>
                                    <br />
                                    <span id="spanOrderCust" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>

                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Customer Note :</span>
                                    <br />
                                    <span id="spanNote" style="font-size:small;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Status Order :</span>
                                    <br />
                                    <span id="spanStatusOrder" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Delivery / Pick Up :</span>
                                    <br />
                                    <span id="spanDelivery" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Production :</span>
                                    <br />
                                    <span id="spanProduction" style="font-size:larger;font-weight:bold;">-</span>
                                </div>

                                <div class="col-lg-8">
                                    <span style="font-size:larger;">Status Note :</span>
                                    <br />
                                    <span id="spanStatusNote" style="font-size:small;font-weight:bold;"></span>
                                </div>


                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Created By :</span>
                                    <br />
                                    <span id="spanCreatedBy" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Created Date :</span>
                                    <br />
                                    <span id="spanCreatedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Submitted Date :</span>
                                    <br />
                                    <span id="spanSubmittedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Production Date :</span>
                                    <br />
                                    <span id="spanProductionDate" style="font-size:larger;font-weight:bold;cursor: pointer;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Completed Date :</span>
                                    <br />
                                    <span id="spanCompletedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Canceled Date :</span>
                                    <br />
                                    <span id="spanCanceledDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                            <div class="row mb-4">
                                 <div class="col-lg-6">
                                    <span style="font-size:larger;">Job Number :</span>
                                    <br />
                                    <div  id="spanJoNumber" style="font-size: larger; cursor: pointer;"></div>
                                    <div  id="spanJoNumberMsg"  class="fst-italic text-secondary">Copied</div>
                                </div>
                                 <div class="col-lg-6">
                                    <span style="font-size:larger;">Order Product Type :</span>
                                    <br />
                                    <div  id="spanOrderProductType" style="font-size: larger;font-weight: bold;" ></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div><!-- /card information header 1 -->

            <!-- card information header 2-->
            <div class="row mb-3">
                <div class="col-lg-6" id="divDate">
                    <div class="card">
                        <div class="card-body border-bottom pb-1">
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Shipment # :</span>
                                    <br />
                                    <div id="spanShipment" style="font-size:larger;font-weight:bold;">-</div>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">ETA to Port :</span>
                                    <br />
                                    <div id="spanEtaPort" style="font-size:larger;font-weight:bold;">-</div>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">ETA to Customer :</span>
                                    <br />
                                    <div id="spanEtaCustomer" style="font-size:larger;font-weight:bold;">-</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6" id="divPrice">
                    <div class="card">
                        <div class="card-body border-bottom pb-1" id="cardPrice">
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Total excl. GST :</span>
                                    <br />
                                    <div  id="spanTotal"></div>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">GST :</span>
                                    <br />
                                    <div  id="spanGST" ></div>
                                </div>

                                 <div class="col-lg-4">
                                    <span style="font-size:larger;">TOTAL incl. GST :</span>
                                    <br />
                                    <div  id="spanFinalTotal" ></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div><!-- /card information header 2 -->

            <!-- card table data items-->
            <div class="row mb-3">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">YOUR ITEMS</h3>
                            <div class="card-actions">
                                <a href="javascript:void(0);" id="btnAddItem" class="btn btn-primary ">
                                    <i class="ti ti-clipboard-plus fs-2 me-2 opacity-50"></i> Add Item
                                </a>
                                <a href="javascript:void(0);" id="btnAddSurcharge" class="btn btn-primary ">
                                    <i class="ti ti-database-dollar fs-2 me-2 opacity-50"></i> Add Surcharge
                                </a>
                            </div>
                        </div>
                        <div class="card-body">
                            <div id="table-default" class="">
                                <table class="table table-bordered table-vcenter table-hover w-100" id="tableAjax">
                                 <thead>
                                     <tr>
                                         <th class="text-center">#</th>
                                         <th class="h3 text-center">ITEM ID</th>
                                         <th class="h3 text-center">QTY</th>
                                         <th class="h3">LOCATION</th>
                                         <th class="h3">PRODUCT</th>
                                         <th class="h3 thPrice">COST</th>
                                         <th class="h3 thMarkUp" >MARK UP</th>
                                         <th class="h3 text-center">ACTIONS</th>
                                     </tr>
                                 </thead>
                                 <tbody></tbody>
                                </table>
                            </div>
                        </div>
                        <div class="card-footer">
                            <p id="msgThanks" class="text-secondary text-center">Thank you for submitting your order. Your order will be processed within 1 business day.<br /> Once your order has been processed, you can check the status from web order.<br /> Please do not fax us any paper work in addition to this online order as it may result in duplication.</p>
                        </div>
                    </div>
                </div>
            </div><!-- /card table data items-->

        </div>
    </div><!-- /page-body -->

    <!-- modalChangeStatus -->
    <div class="modal fade" id="modalChangeStatus" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalChangeStatusLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalChangeStatusLabel">Update Status Order</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body row">

                    <div class="col-12 mb-3">
                        <label for="status" class="form-label text-uppercase">Status</label>
                        <select name="status" id="status" class="form-select"></select>

                        <input type="text" class="form-control " id="statusOld" name="statusOld"readonly hidden>

                        <input type="text" class="form-control " id="id" name="id" placeholder="id for edit" readonly hidden>
                    </div>

                    <div class="col-12 mb-3" id="divSubmittedDate">
                        <label for="submitteddate" class="form-label text-uppercase">Submitted Date</label>
                        <input type="date" class="form-control" id="submitteddate" name="submitteddate">
                    </div>

                    <div class="col-12 mb-3" id="divCompletedDate">
                        <label for="completeddate" class="form-label text-uppercase">Completed Date</label>
                        <input type="date" class="form-control " id="completeddate" name="completeddate">
                    </div>

                    <div class="col-12 mb-3" id="divCanceledDate">
                        <label for="canceleddate" class="form-label text-uppercase">Canceled Date</label>
                        <input type="date" class="form-control " id="canceleddate" name="canceleddate">
                    </div>

                    <div class="col-12 mb-3" id="divDescription">
                        <label for="canceleddate" class="form-label text-uppercase d-flex justify-content-between">
                            Description
                            <i class="bi bi-question-circle" style="cursor: pointer;" id="tooltipDescription"></i>
                        </label>
                        <textarea name="description" class="form-control" id="description" rows="3" ></textarea>
                    </div>


                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    <button type="button" class="btn btn-primary" id="submitChangeStatus">
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i>Submit
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Override Discount-->
    <div class="modal fade" id="modalQuoteDisc" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalQuoteDiscLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalQuoteDiscLabel">Override Customer Discount (%)</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row ">
                        <div class="col-lg-10 mx-auto">
                            <label for="notes" class="text-uppercase fw-bold mb-2">Discount</label>
                            <div class="input-group">
                                <input type="number" min="0" name="discount" id="discount" class="form-control "  autocomplete="off">
                                <span class="input-group-text ">%</span>
                            </div>
                            <small class="form-hint text-warning">* For this order only (including all products) !</small>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="btnSubmitOverrideDisc">Save Changes</button>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="modalProductionDate" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalProductionDateLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalProductionDateLabel">Change Production Date</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="col-12">
                        <label for="productiondate" class="form-label text-uppercase">Production Date</label>
                        <input type="date" class="form-control" id="productiondate" name="productiondate">
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="btnSubmitProductionDate">Save Changes</button>
                </div>
            </div>
        </div>
    </div>


    <!-- modalAddItem -->
    <div class="modal fade" id="modalAddItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalAddItemLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalAddItemLabel">Add Item</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body row">
                    <div class="col-12">
                        <label for="designid" class="form-label text-uppercase">select product</label>
                        <select name="designid" id="designid" class="form-select"></select>
                        <!-- <small class="form-hint" style="color:red;">* Please select a product then click the submit button</small> -->
                    </div>
                    <div class="col-12 mt-3" id="divProduction" >
                        <label for="designid" class="form-label text-uppercase">production</label>
                        <select name="production" id="production" class="form-select"></select>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                        Close
                    </button>
                    <button type="button" class="btn btn-primary" id="submitAddItem">
                        Next
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- modalAddService -->
    <div class="modal fade" id="modalAddService" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalAddServiceLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalAddServiceLabel">Add new surcharge</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 col-lg-12 col-md-12 mb-3">
                            <label for="category" class="form-label text-uppercase required">category</label>
                            <select name="category" id="category" class="form-select"></select>
                            <input type="text" name="id" id="id" class="form-control" hidden readonly>
                        </div>
                        <div class="col-12 col-lg-12 col-md-12 mb-3" id="divType">
                            <label for="type" class="form-label text-uppercase required" id="lblType">category type</label>
                            <select name="type" id="type" class="form-select"></select>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="btnSubmitService">Submit</button>
                </div>
            </div>
        </div>
    </div>

    

    <!-- modalSendMailQuote -->
    <div class="modal fade" id="modalSendMailQuote" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalSendMailQuoteLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title " id="modalSendMailQuoteLabel">Send Mail Quote</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 col-lg-12 col-md-12 mb-3">
                            <label for="from" class="form-label text-uppercase required">from</label>
                            <input type="text" name="from" id="from" class="form-control" readonly>
                            <input type="text" name="id" id="id" class="form-control"  hidden>
                        </div>
                        <div class="col-12 col-lg-12 col-md-12 mb-3">
                            <label for="mailto" class="form-label text-uppercase required">to</label>
                            <input type="text" name="mailto" id="mailto" class="form-control" >
                            <small class="form-hint">This is taken from customers primary contact email address if available.</small>
                        </div>
                        <div class="col-12 col-lg-12 col-md-12 mb-3">
                            <label for="cc" class="form-label text-uppercase required">cc</label>
                            <input type="text" name="cc" id="cc" class="form-control" >
                            <small class="form-hint">Will be cc'ed to <b>Customer Service</b> and <b>Accounts</b> by default.</small>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="btnSendMailQuote">Send</button>
                </div>
            </div>
        </div>
    </div>

    <!-- modalEditPricingItem -->
    <div class="modal fade" id="modalEditPricingItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalEditPricingItemLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title " id="modalEditPricingItemLabel">Edit Pricing</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class=" col-4 col-lg-4 col-md-4">
                            <label for="" class="form-label">Base Price</label>
                             <div class="input-group ">
                                <span class="input-group-text">$</span>
                                <input type="text" class="form-control" id="cost" name="cost" readonly>
                            </div>
                            <input type="text" class="form-control" id="id" name="id" readonly hidden>
                            <input type="text" class="form-control" id="designid" name="designid" readonly hidden>
                            <input type="text" class="form-control" id="blindid" name="blindid" readonly hidden>
                        </div>
                        <div class="col-8 col-lg-8 col-md-8">
                            <label for="" class="form-label">Override Price</label>
                            <div class="input-group ">
                                <span class="input-group-text">$</span>
                                <input type="number" class="form-control" id="newcost" name="newcost" autocomplete="off" placeholder="Example : 107.65">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="submitEditPricingItem">Save Changes</button>
                </div>
            </div>
        </div>
    </div>

    <!-- modalEditPricingAllItem -->
    <div class="modal fade" id="modalEditPricingAllItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalEditPricingAllItemLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title " id="modalEditPricingAllItemLabel">Edit Pricing</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body" id="modalBody">
                    
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-primary" id="submitEditPricingAllItem">Save Changes</button>
                </div>
            </div>
        </div>
    </div>

    <!-- modalPricingItem -->
    <div class="modal fade" id="modalPricingItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalPricingItemLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalPricingItemLabel">Cost Details</h1>
                    <button type="button" class="btn-close close-button" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">

                    <div id="table-default">
                        <table class="table table-bordered" id="tablePricingDetail" width="100%">
                            <thead class="fs-3 fw-bold">
                                <tr>
                                    <th class="text-center">#</th>
                                    <th class="h3 text-center">Qty</th>
                                    <th class="h3">Descpription</th>
                                    <th class="h3">Cost / Qty</th>
                                    <th class="h3">POA / Qty</th>
                                    <th class="h3">Discount / Qty</th>
                                    <th class="h3 ">Sub Total</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary close-button" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- logs -->
    <div class="modal fade" id="modalLogs" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalLogsLabel" aria-hidden="true">
        <div class="modal-dialog  modal-dialog-centered modal-dialog-scrollable">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalLogsLabel">Changelog</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <table class="table table-bordered" width="100%" id="table-logs">
                        <tbody></tbody>
                        <!-- <tr>
                            <td>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</td>
                        </tr>
                        <tr>
                            <td>Lorem ipsum dolor sit, amet consectetur adipisicing elit.</td>
                        </tr> -->
                    </table>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>

    <br>
    
    

    <script type="text/javascript">
        const params = new URLSearchParams(window.location.search);
        const ULTRON = params.get("param"); // AS HeaderId
        const INFYNITY = params.get("ordertype"); // AS OrderType

        let HEADERID = ULTRON;
        let ORDERTYPE = INFYNITY;
        let CUSTOMERID = '<%= Session("CustomerId") %>';
        let CUSTOMERCOMPANY = '<%= Session("CustomerCompany") %>';
        let CUSTOMERCONTACTID = '<%= Session("CustomerContactId") %>';
        let USERID = '<%= Session("userId") %>';
        let USERNAME = '<%= Session("UserName") %>';
        let LOGINID = '<%= Session("LoginId") %>';
        let APPLICATIONID = '<%= Session("ApplicationId") %>';
        let ROLENAME = '<%= Session("RoleName") %>';  
        let PRICEACCESS = '<%= Session("PriceAccess") %>';
        let MARKUPACCESS = '<%= Session("MarkUpAccess") %>';
        let PREVIEWACCESS = '<%= Session("printPreview") %>';
        let REPRINT = '<%= Session("Reprint") %>';
        let URIMETHOD = '/Methods/Order/OrderDetailMethod.aspx';      
        let PDFORDERMETHOD = '/Methods/Order/PdfOrderMethod.aspx';      
        let JOBSHEETMETHOD = '/Methods/Order/JobSheetMethod.aspx';      
    </script>
    <script src="/Scripts/Order/OrderDetails.js?v=1.0.16"></script>


</asp:Content>

