<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Additional.aspx.vb"
Inherits="Order_Additional" MasterPageFile="~/Site.Master" Title="Additional" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
  <style>
    .datagridTitle {
      font-size: 14px;
    }
    .datagridContent {
      font-size: 16px;
    }
  </style>
  <div class="page-header">
    <div class="container-xl">
      <div class="row g-2 align-items-center">
        <div class="col">
          <div class="page-pretitle">
            <span>ORDER</span>
          </div>
          <h2 class="page-title" id="pageTitle"></h2>
        </div>
      </div>
    </div>
  </div>

  <div class="page-body">
    <div class="container-xl" id="divOrder" style="display: none">
      <div class="row mb-3">
        <div class="col-lg-8 col-md-12 col-sm-12">
          <div class="card">
            <div class="card-body">
              <div class="datagrid">
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Order #</div>
                  <div class="datagrid-content datagridContent">
                    <span id="orderid"></span>
                  </div>
                </div>
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Order Number</div>
                  <div class="datagrid-content datagridContent">
                    <span id="orderno"></span>
                  </div>
                </div>
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Customer Name</div>
                  <div class="datagrid-content datagridContent">
                    <span id="ordername"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col-lg-8 col-md-12 col-sm-12">
          <div class="card">
            <div class="card-header">
              <h3 class="card-title" id="cardTitle"></h3>
            </div>
            <div class="card-body">
              <div class="mb-3 row">
                <label class="col-lg-3 col-form-label required">CATEGORY</label>
                <div class="col-lg-5 col-md-12 col-sm-12">
                  <select
                    class="form-select"
                    id="blindtype"
                    style="font-weight: bold"
                  ></select>
                </div>
              </div>

              <div class="mb-6 row" id="divType">
                <label
                  class="col-lg-3 col-form-label required"
                  id="producttitle"
                  >TYPE</label
                >
                <div class="col-lg-7 col-md-12 col-sm-12">
                  <select class="form-select" id="product"></select>
                </div>
              </div>

              <div class="mb-3 row" id="divItemName">
                <label class="col-lg-3 col-form-label required"
                  >ITEM NAME</label
                >
                <div class="col-lg-8 col-md-12 col-sm-12">
                  <input
                    id="itemname"
                    class="form-control"
                    autocomplete="off"
                    placeholder="Item Name ...."
                  />
                </div>
              </div>

              <div class="mb-3 row" id="divItemCode">
                <label class="col-lg-3 col-form-label">ITEM CODE</label>
                <div class="col-lg-6 col-md-12 col-sm-12">
                  <input
                    id="itemcode"
                    class="form-control"
                    autocomplete="off"
                    placeholder="Item Code ...."
                  />
                </div>
              </div>

              <div class="mb-3 row" id="divQty">
                <label class="col-lg-3 col-form-label required">Qty</label>
                <div class="col-lg-2 col-md-12 col-sm-12">
                  <input
                    type="number"
                    id="qty"
                    class="form-control"
                    autocomplete="off"
                    placeholder="Qty ...."
                  />
                </div>
              </div>

              <div class="mb-3 row" id="divItemNumber">
                <label class="col-lg-3 col-form-label required"
                  >NUMBER OF ITEM</label
                >
                <div class="col-lg-3 col-md-12 col-sm-12">
                  <input
                    type="number"
                    id="itemnumber"
                    class="form-control"
                    autocomplete="off"
                    placeholder="Item ...."
                  />
                </div>
              </div>

              <div class="mb-3 row" id="divCost">
                <label
                  id="costtitle"
                  class="col-lg-3 col-form-label required"
                ></label>
                <div class="col-lg-3 col-md-12 col-sm-12">
                  <div class="input-group">
                    <span class="input-group-text">$</span>
                    <input
                      id="cost"
                      class="form-control"
                      autocomplete="off"
                      placeholder="Cost ...."
                    />
                  </div>
                </div>
              </div>
            </div>
            <div class="card-footer text-center">
              <a href="javascript:void(0);" id="submit" class="btn btn-primary"
                >Submit</a
              >
              <a href="javascript:void(0);" id="cancel" class="btn btn-danger"
                >Cancel</a
              >
            </div>
          </div>
        </div>

        <div class="col-lg-4 col-md-12 col-sm-12">
          <div class="card">
            <div class="card-header">
              <h3 class="card-title">Notes</h3>
            </div>

            <div class="card-body">
              <div class="mb-3 row">
                <p></p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalSuccess"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
      <div class="modal-content">
        <div class="modal-status bg-success"></div>
        <div class="modal-body text-center py-4">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="icon mb-2 text-green icon-lg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            stroke-width="2"
            stroke="currentColor"
            fill="none"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path stroke="none" d="M0 0h24v24H0z" fill="none" />
            <path d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0" />
            <path d="M9 12l2 2l4 -4" />
          </svg>
          <h3>Successfully</h3>
          <div class="text-secondary">
            Your order has been successfully saved
          </div>
        </div>
        <div class="modal-footer">
          <div class="w-100">
            <div class="row">
              <div class="col">
                <a
                  href="javascript:void(0);"
                  id="vieworder"
                  class="btn btn-success w-100"
                  data-bs-dismiss="modal"
                  >View Order</a
                >
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalError"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
      <div class="modal-content">
        <button
          type="button"
          class="btn-close"
          data-bs-dismiss="modal"
          aria-label="Close"
        ></button>
        <div class="modal-status bg-danger"></div>
        <div class="modal-body text-center py-4">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            class="icon mb-2 text-danger icon-lg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            stroke-width="2"
            stroke="currentColor"
            fill="none"
            stroke-linecap="round"
            stroke-linejoin="round"
          >
            <path stroke="none" d="M0 0h24v24H0z" fill="none" />
            <path
              d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z"
            />
            <path d="M12 9v4" />
            <path d="M12 17h.01" />
          </svg>
          <h3>Important Message</h3>
          <div class="text-secondary" id="errorMsg"></div>
        </div>
        <div class="modal-footer">
          <div class="w-100">
            <div class="row">
              <div class="col">
                <a href="#" class="btn btn-danger w-100" data-bs-dismiss="modal"
                  >Close</a
                >
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <script type="text/javascript">
    let designIdOri = "6C0B3347-9730-45CA-905C-5EF682CD06EA";
    let headerId = '<%= Session("headerId") %>';
    let orderType = '<%= Session("orderType") %>';
    let itemAction = '<%= Session("itemAction") %>';
    let designId = '<%= Session("designId") %>';
    let itemId = '<%= Session("itemId") %>';
    let loginId = '<%= Session("LoginId") %>';
  </script>
  <script
    src="../Scripts/Order/Additional.js?v=<%= DateTime.Now.Ticks %>"
  ></script>
</asp:Content>
