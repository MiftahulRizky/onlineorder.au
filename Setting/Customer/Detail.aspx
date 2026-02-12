<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb"
Inherits="Setting_Customer_Detail" MasterPageFile="~/Site.Master"
MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Detail" %>

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
        <div class="col-6">
          <div class="page-pretitle">Setting</div>
          <h2 class="page-title">
            <a runat="server" href="~/setting/customer">Customer</a>
          </h2>
        </div>

        <div class="col-6 text-end">
          <asp:Button
            runat="server"
            ID="btnEdit"
            CssClass="btn btn-info"
            Text="Edit"
            OnClick="btnEdit_Click"
          />
          <a
            href="#"
            runat="server"
            id="aDelete"
            class="btn btn-danger"
            data-bs-toggle="modal"
            data-bs-target="#modalDelete"
            >Delete</a
          >
          <a
            hidden
            href="#"
            runat="server"
            id="aCreateOrder"
            class="btn btn-orange"
            data-bs-toggle="modal"
            data-bs-target="#modalCreateOrder"
            >Create Order</a
          >
          <a
            href="#"
            runat="server"
            id="aLog"
            class="btn btn-secondary"
            data-bs-toggle="modal"
            data-bs-target="#modalLog"
            >Log</a
          >
        </div>
      </div>
    </div>
  </div>

  <div class="page-body">
    <div class="container-xl">
      <div class="row" runat="server" id="divError">
        <div class="col-12">
          <div
            class="alert alert-important alert-danger alert-dismissible"
            role="alert"
          >
            <div class="d-flex">
              <div>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  class="icon alert-icon"
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
                  <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                  <path d="M12 8v4" />
                  <path d="M12 16h.01" />
                </svg>
              </div>
              <div>
                <span runat="server" id="msgError"></span>
              </div>
            </div>
            <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
          </div>
        </div>
      </div>

      <div class="row mb-3">
        <div class="col-12">
          <div class="card">
            <div class="card-header">
              <h3 class="card-title">Detail Customer</h3>
            </div>
            <div class="card-body">
              <div class="datagrid">
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Customer Name</div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblName"></asp:Label>
                  </div>
                </div>
              </div>

              <br /><br />

              <div class="datagrid">
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Account</div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblAccount"></asp:Label>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">
                    Master Customer
                  </div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblMasterId"></asp:Label>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Customer Type</div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblType"></asp:Label>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Sales Person</div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblSalesPerson"></asp:Label>
                  </div>
                </div>
              </div>

              <br /><br />

              <div class="datagrid">
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Web Id</div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblId"></asp:Label>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Exact ID</div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblExactId"></asp:Label>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Customer Group</div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblGroup"></asp:Label>
                  </div>
                </div>
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">
                    Customer Price Group
                  </div>
                  <div class="datagrid-content datagridContent">
                    <asp:Label runat="server" ID="lblPriceGroup"></asp:Label>
                  </div>
                </div>
              </div>

              <br /><br />

              <div class="datagrid">
                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">On Stop</div>
                  <div class="datagrid-content">
                    <div runat="server" id="divOnStopGreen">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M9 12l2 2l4 -4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      Yes
                    </div>
                    <div runat="server" id="divOnStopDanger">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M10 10l4 4m0 -4l-4 4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      No
                    </div>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Cash Sale</div>
                  <div class="datagrid-content">
                    <div runat="server" id="divCashSaleGreen">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M9 12l2 2l4 -4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      Yes
                    </div>
                    <div runat="server" id="divCashSaleDanger">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M10 10l4 4m0 -4l-4 4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      No
                    </div>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">Newsletter</div>
                  <div class="datagrid-content">
                    <div runat="server" id="divNewsletterGreen">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M9 12l2 2l4 -4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      Yes
                    </div>
                    <div runat="server" id="divNewsletterDanger">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M10 10l4 4m0 -4l-4 4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      No
                    </div>
                  </div>
                </div>

                <div class="datagrid-item">
                  <div class="datagrid-title datagridTitle">
                    Min Order Surcharge
                  </div>
                  <div class="datagrid-content">
                    <div runat="server" id="divMinChargeGreen">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M9 12l2 2l4 -4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      Yes
                    </div>
                    <div runat="server" id="divMinChargeDanger">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger"
                      >
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M10 10l4 4m0 -4l-4 4" />
                        <path
                          d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z"
                        />
                      </svg>
                      No
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="row mb-3">
        <div class="col-12">
          <div class="card" id="dvTab">
            <div class="card-header">
              <ul
                class="nav nav-tabs card-header-tabs nav-fill"
                data-bs-toggle="tabs"
              >
                <li class="nav-item" runat="server" id="liContact">
                  <a
                    href="#tabsContact"
                    class="nav-link active"
                    data-bs-toggle="tab"
                    id="linkContact"
                    >CONTACTS</a
                  >
                </li>
                <li class="nav-item" runat="server" id="liAddress">
                  <a
                    href="#tabsAddress"
                    class="nav-link"
                    data-bs-toggle="tab"
                    id="linkAddress"
                    >ADDRESSES</a
                  >
                </li>
                <li class="nav-item" runat="server" id="liLogin">
                  <a
                    href="#tabsLogin"
                    class="nav-link"
                    data-bs-toggle="tab"
                    id="linkLogin"
                    >LOGINS</a
                  >
                </li>
                <li class="nav-item" runat="server" id="liDiscount">
                  <a
                    href="#tabsDiscount"
                    class="nav-link"
                    data-bs-toggle="tab"
                    id="linkDiscount"
                    >DISCOUNTS</a
                  >
                </li>
                <li class="nav-item" runat="server" id="liAccess">
                  <a
                    href="#tabsAccess"
                    class="nav-link"
                    data-bs-toggle="tab"
                    id="linkAccess"
                    >PRODUCT ACCESS</a
                  >
                </li>
                <li class="nav-item" runat="server" id="liQuote">
                  <a
                    href="#tabsQuote"
                    class="nav-link"
                    data-bs-toggle="tab"
                    id="linkQuote"
                    >QUOTES</a
                  >
                </li>
              </ul>
            </div>

            <div class="card-body">
              <div class="tab-content">
                <div class="tab-pane active show" id="tabsContact">
                  <div class="row mb-3" runat="server" id="divErrorContact">
                    <div class="col-12">
                      <div
                        class="alert alert-important alert-danger alert-dismissible"
                        role="alert"
                      >
                        <div class="d-flex">
                          <div>
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              class="icon alert-icon"
                              width="24"
                              height="24"
                              viewBox="0 0 24 24"
                              stroke-width="2"
                              stroke="currentColor"
                              fill="none"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                            >
                              <path
                                stroke="none"
                                d="M0 0h24v24H0z"
                                fill="none"
                              />
                              <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                              <path d="M12 8v4" />
                              <path d="M12 16h.01" />
                            </svg>
                          </div>
                          <div>
                            <span runat="server" id="msgErrorContact"></span>
                          </div>
                        </div>
                        <a
                          class="btn-close"
                          data-bs-dismiss="alert"
                          aria-label="close"
                        ></a>
                      </div>
                    </div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-12">
                      <div class="table-responsive">
                        <asp:GridView
                          runat="server"
                          ID="gvListContact"
                          CssClass="table table-vcenter card-table"
                          AutoGenerateColumns="false"
                          ShowHeaderWhenEmpty="true"
                          AllowPaging="true"
                          PageSize="10"
                          EmptyDataText="CONTACT NOT FOUND :)"
                          EmptyDataRowStyle-HorizontalAlign="Center"
                          PagerSettings-Position="TopAndBottom"
                          OnPageIndexChanging="gvListContact_PageIndexChanging"
                          OnRowCommand="gvListContact_RowCommand"
                        >
                          <RowStyle />
                          <Columns>
                            <asp:TemplateField
                              HeaderText="#"
                              ItemStyle-Width="60px"
                            >
                              <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField
                              DataField="Id"
                              HeaderStyle-CssClass="hiddencol"
                              ItemStyle-CssClass="hiddencol"
                            />
                            <asp:BoundField
                              DataField="Name"
                              HeaderText="Name"
                            />
                            <asp:BoundField
                              DataField="Salutation"
                              HeaderText="Salutation"
                            />
                            <asp:BoundField
                              DataField="Role"
                              HeaderText="Role"
                            />
                            <asp:BoundField
                              DataField="Email"
                              HeaderText="Email"
                            />
                            <asp:BoundField
                              DataField="Phone"
                              HeaderText="Phone"
                            />
                            <asp:BoundField
                              DataField="Mobile"
                              HeaderText="Mobile"
                            />
                            <asp:BoundField
                              DataField="Tags"
                              HeaderText="Tags"
                            />
                            <asp:BoundField
                              DataField="Note"
                              HeaderText="Note"
                            />
                            <asp:TemplateField HeaderText="Primary">
                              <ItemTemplate>
                                <div
                                  runat="server"
                                  visible='<%# VisiblePrimaryYes(Eval("Primary")) %>'
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    class="icon icon-tabler icons-tabler-outline text-success icon-tabler-circle-check"
                                  >
                                    <path
                                      stroke="none"
                                      d="M0 0h24v24H0z"
                                      fill="none"
                                    />
                                    <path
                                      d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0"
                                    />
                                    <path d="M9 12l2 2l4 -4" />
                                  </svg>
                                </div>
                                <div
                                  runat="server"
                                  visible='<%# VisiblePrimaryNo(Eval("Primary")) %>'
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    class="icon icon-tabler icons-tabler-outline text-danger icon-tabler-circle-x"
                                  >
                                    <path
                                      stroke="none"
                                      d="M0 0h24v24H0z"
                                      fill="none"
                                    />
                                    <path
                                      d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0"
                                    />
                                    <path d="M10 10l4 4m0 -4l-4 4" />
                                  </svg>
                                </div>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120px">
                              <ItemTemplate>
                                <button
                                  class="btn btn-sm btn-pill btn-purple"
                                  data-bs-toggle="dropdown"
                                  runat="server"
                                  visible="<%# VisibleActionContact() %>"
                                >
                                  Actions
                                </button>
                                <div class="dropdown-menu dropdown-menu-end">
                                  <asp:LinkButton
                                    runat="server"
                                    ID="linkDetailContact"
                                    CssClass="dropdown-item"
                                    Text="Detail / Edit"
                                    CommandName="Detail"
                                    CommandArgument='<%# Eval("Id") %>'
                                  ></asp:LinkButton>
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalDeleteContact"
                                    onclick='<%# String.Format("return showDeleteContact(`{0}`);", Eval("Id").ToString()) %>'
                                    >Delete</a
                                  >
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalPrimaryContact"
                                    onclick='<%# String.Format("return showPrimaryContact(`{0}`);", Eval("Id").ToString()) %>'
                                    visible='<%# VisiblePrimaryContact(Eval("Primary")) %>'
                                    >Set As Primary Contact</a
                                  >
                                  <div class="dropdown-divider"></div>
                                  <asp:LinkButton
                                    runat="server"
                                    CssClass="dropdown-item"
                                    ID="linkLogContact"
                                    Text="Logs"
                                    CommandName="Log"
                                    CommandArgument='<%# Eval("Id") %>'
                                  ></asp:LinkButton>
                                </div>
                              </ItemTemplate>
                            </asp:TemplateField>
                          </Columns>
                          <PagerStyle
                            BackColor="DodgerBlue"
                            ForeColor="White"
                            HorizontalAlign="Center"
                          />
                          <PagerSettings
                            PreviousPageText="Prev"
                            NextPageText="Next"
                            Mode="NumericFirstLast"
                          />
                          <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                      </div>
                    </div>
                  </div>

                  <div class="row text-start">
                    <div class="col-12">
                      <asp:Button
                        runat="server"
                        ID="btnAddContact"
                        Text="New Contact"
                        CssClass="btn btn-primary"
                        OnClick="btnAddContact_Click"
                      />
                      <a
                        href="#"
                        runat="server"
                        id="aResetContact"
                        class="btn btn-danger"
                        data-bs-toggle="modal"
                        data-bs-target="#modalResetPrimaryContact"
                        >Reset Primary Contact</a
                      >
                    </div>
                  </div>
                </div>

                <div class="tab-pane" id="tabsAddress">
                  <div class="row mb-3" runat="server" id="divErrorAddress">
                    <div class="col-12">
                      <div
                        class="alert alert-important alert-danger alert-dismissible"
                        role="alert"
                      >
                        <div class="d-flex">
                          <div>
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              class="icon alert-icon"
                              width="24"
                              height="24"
                              viewBox="0 0 24 24"
                              stroke-width="2"
                              stroke="currentColor"
                              fill="none"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                            >
                              <path
                                stroke="none"
                                d="M0 0h24v24H0z"
                                fill="none"
                              />
                              <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                              <path d="M12 8v4" />
                              <path d="M12 16h.01" />
                            </svg>
                          </div>
                          <div>
                            <span runat="server" id="msgErrorAddress"></span>
                          </div>
                        </div>
                        <a
                          class="btn-close"
                          data-bs-dismiss="alert"
                          aria-label="close"
                        ></a>
                      </div>
                    </div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-12">
                      <div class="table-responsive">
                        <asp:GridView
                          runat="server"
                          ID="gvListAddress"
                          CssClass="table table-vcenter card-table"
                          AutoGenerateColumns="false"
                          AllowPaging="true"
                          ShowHeaderWhenEmpty="true"
                          EmptyDataText="ADDRESS NOT FOUND :)"
                          EmptyDataRowStyle-HorizontalAlign="Center"
                          PageSize="10"
                          PagerSettings-Position="TopAndBottom"
                          OnPageIndexChanging="gvListAddress_PageIndexChanging"
                          OnRowCommand="gvListAddress_RowCommand"
                        >
                          <RowStyle />
                          <Columns>
                            <asp:TemplateField
                              HeaderText="#"
                              ItemStyle-Width="60px"
                            >
                              <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField
                              DataField="Id"
                              HeaderStyle-CssClass="hiddencol"
                              ItemStyle-CssClass="hiddencol"
                            />
                            <asp:BoundField
                              DataField="Description"
                              HeaderText="Description"
                            />
                            <asp:TemplateField HeaderText="Address">
                              <ItemTemplate>
                                <%# BindDetailAddress(Eval("Id").ToString()) %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField
                              DataField="Port"
                              HeaderText="Nearest Port"
                            />
                            <asp:BoundField
                              DataField="Tags"
                              HeaderText="Tags"
                            />
                            <asp:BoundField
                              DataField="Instruction"
                              HeaderText="Instruction"
                            />
                            <asp:TemplateField HeaderText="Primary">
                              <ItemTemplate>
                                <div
                                  runat="server"
                                  visible='<%# VisiblePrimaryYes(Eval("Primary")) %>'
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    class="icon icon-tabler text-success icons-tabler-outline icon-tabler-circle-check"
                                  >
                                    <path
                                      stroke="none"
                                      d="M0 0h24v24H0z"
                                      fill="none"
                                    />
                                    <path
                                      d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0"
                                    />
                                    <path d="M9 12l2 2l4 -4" />
                                  </svg>
                                </div>

                                <div
                                  runat="server"
                                  visible='<%# VisiblePrimaryNo(Eval("Primary")) %>'
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    class="icon icon-tabler text-danger icons-tabler-outline icon-tabler-circle-x"
                                  >
                                    <path
                                      stroke="none"
                                      d="M0 0h24v24H0z"
                                      fill="none"
                                    />
                                    <path
                                      d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0"
                                    />
                                    <path d="M10 10l4 4m0 -4l-4 4" />
                                  </svg>
                                </div>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120px">
                              <ItemTemplate>
                                <button
                                  class="btn btn-sm btn-pill btn-purple"
                                  data-bs-toggle="dropdown"
                                  runat="server"
                                  visible="<%# VisibleActionAddress() %>"
                                >
                                  Actions
                                </button>
                                <div class="dropdown-menu dropdown-menu-end">
                                  <asp:LinkButton
                                    runat="server"
                                    ID="linkDetailAddress"
                                    CssClass="dropdown-item"
                                    Text="Detail / Edit"
                                    CommandName="Detail"
                                    CommandArgument='<%# Eval("Id") %>'
                                  ></asp:LinkButton>
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalDeleteAddress"
                                    onclick='<%# String.Format("return showDeleteAddress(`{0}`);", Eval("Id").ToString()) %>'
                                    >Delete</a
                                  >
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalPrimaryAddress"
                                    onclick='<%# String.Format("return showPrimaryAddress(`{0}`);", Eval("Id").ToString()) %>'
                                    visible='<%# VisiblePrimaryAddress(Eval("Primary")) %>'
                                    >Set As Primary Address</a
                                  >
                                  <div class="dropdown-divider"></div>
                                  <asp:LinkButton
                                    runat="server"
                                    CssClass="dropdown-item"
                                    ID="linkLogAddress"
                                    Text="Logs"
                                    CommandName="Log"
                                    CommandArgument='<%# Eval("Id") %>'
                                  ></asp:LinkButton>
                                </div>
                              </ItemTemplate>
                            </asp:TemplateField>
                          </Columns>
                          <PagerStyle
                            BackColor="DodgerBlue"
                            ForeColor="White"
                            HorizontalAlign="Center"
                          />
                          <PagerSettings
                            PreviousPageText="Prev"
                            NextPageText="Next"
                            Mode="NumericFirstLast"
                          />
                          <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                      </div>
                    </div>
                  </div>
                  <div class="row text-start">
                    <div class="col-12">
                      <asp:Button
                        runat="server"
                        ID="btnAddAddress"
                        Text="New Address"
                        CssClass="btn btn-primary"
                        OnClick="btnAddAddress_Click"
                      />
                      <a
                        href="#"
                        runat="server"
                        id="aResetPrimaryAddress"
                        class="btn btn-danger"
                        data-bs-toggle="modal"
                        data-bs-target="#modalResetPrimaryAddress"
                        >Reset Primary Address</a
                      >
                    </div>
                  </div>
                </div>

                <div class="tab-pane" id="tabsLogin">
                  <div class="row mb-3" runat="server" id="divErrorLogin">
                    <div class="col-12">
                      <div
                        class="alert alert-important alert-danger alert-dismissible"
                        role="alert"
                      >
                        <div class="d-flex">
                          <div>
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              class="icon alert-icon"
                              width="24"
                              height="24"
                              viewBox="0 0 24 24"
                              stroke-width="2"
                              stroke="currentColor"
                              fill="none"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                            >
                              <path
                                stroke="none"
                                d="M0 0h24v24H0z"
                                fill="none"
                              />
                              <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                              <path d="M12 8v4" />
                              <path d="M12 16h.01" />
                            </svg>
                          </div>
                          <div>
                            <span runat="server" id="msgErrorLogin"></span>
                          </div>
                        </div>
                        <a
                          class="btn-close"
                          data-bs-dismiss="alert"
                          aria-label="close"
                        ></a>
                      </div>
                    </div>
                  </div>
                  <div class="row py-3">
                    <div class="d-flex">
                      <div class="ms-auto text-secondary">
                        <asp:Panel
                          runat="server"
                          DefaultButton="btnSearchLogin"
                        >
                          <div class="ms-2 d-inline-block">
                            <asp:TextBox
                              runat="server"
                              ID="txtSearchLogin"
                              CssClass="form-control"
                              placeholder="Search Data"
                              autocomplete="off"
                            ></asp:TextBox>
                          </div>
                          <div class="ms-2 d-inline-block">
                            <asp:Button
                              runat="server"
                              ID="btnSearchLogin"
                              CssClass="btn btn-primary"
                              Text="Search"
                              OnClick="btnSearchLogin_Click"
                            />
                          </div>
                        </asp:Panel>
                      </div>
                    </div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-12">
                      <div class="table-responsive">
                        <asp:GridView
                          runat="server"
                          ID="gvListLogin"
                          CssClass="table table-vcenter card-table"
                          AutoGenerateColumns="false"
                          AllowPaging="true"
                          ShowHeaderWhenEmpty="true"
                          EmptyDataText="LOGIN NOT FOUND :)"
                          EmptyDataRowStyle-HorizontalAlign="Center"
                          PageSize="50"
                          PagerSettings-Position="TopAndBottom"
                          OnPageIndexChanging="gvListLogin_PageIndexChanging"
                          OnRowCommand="gvListLogin_RowCommand"
                        >
                          <RowStyle />
                          <Columns>
                            <asp:BoundField
                              DataField="Id"
                              HeaderStyle-CssClass="hiddencol"
                              ItemStyle-CssClass="hiddencol"
                            />
                            <asp:BoundField
                              DataField="AppName"
                              HeaderText="Application"
                            />
                            <asp:BoundField
                              DataField="RoleName"
                              HeaderText="Role"
                            />
                            <asp:BoundField
                              DataField="UserName"
                              HeaderText="User"
                            />
                            <asp:BoundField
                              DataField="FullName"
                              HeaderText="Full Name"
                            />
                            <asp:BoundField
                              DataField="LastLogin"
                              HeaderText="Last Login"
                              DataFormatString="{0:dd MMM yyyy HH:mm:ss}"
                            />
                            <asp:TemplateField HeaderText="Active">
                              <ItemTemplate>
                                <div
                                  runat="server"
                                  visible='<%# VisiblePrimaryYes(Eval("Active")) %>'
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    class="icon icon-tabler icons-tabler-outline text-success icon-tabler-circle-check"
                                  >
                                    <path
                                      stroke="none"
                                      d="M0 0h24v24H0z"
                                      fill="none"
                                    />
                                    <path
                                      d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0"
                                    />
                                    <path d="M9 12l2 2l4 -4" />
                                  </svg>
                                </div>
                                <div
                                  runat="server"
                                  visible='<%# VisiblePrimaryNo(Eval("Active")) %>'
                                >
                                  <svg
                                    xmlns="http://www.w3.org/2000/svg"
                                    width="24"
                                    height="24"
                                    viewBox="0 0 24 24"
                                    fill="none"
                                    stroke="currentColor"
                                    stroke-width="2"
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    class="icon icon-tabler icons-tabler-outline text-danger icon-tabler-circle-x"
                                  >
                                    <path
                                      stroke="none"
                                      d="M0 0h24v24H0z"
                                      fill="none"
                                    />
                                    <path
                                      d="M12 12m-9 0a9 9 0 1 0 18 0a9 9 0 1 0 -18 0"
                                    />
                                    <path d="M10 10l4 4m0 -4l-4 4" />
                                  </svg>
                                </div>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField
                              DataField="Active"
                              HeaderStyle-CssClass="hiddencol"
                              ItemStyle-CssClass="hiddencol"
                            />
                            <asp:TemplateField ItemStyle-Width="120px">
                              <ItemTemplate>
                                <button
                                  class="btn btn-sm btn-pill btn-purple"
                                  data-bs-toggle="dropdown"
                                  runat="server"
                                  visible='<%# VisibleActions_Login(Eval("CustomerId").ToString()) %>'
                                >
                                  Actions
                                </button>
                                <div class="dropdown-menu dropdown-menu-end">
                                  <asp:LinkButton
                                    runat="server"
                                    CssClass="dropdown-item"
                                    ID="linkDetailLogin"
                                    Text="Detail / Edit"
                                    CommandName="Detail"
                                    CommandArgument='<%# Eval("Id") %>'
                                  ></asp:LinkButton>
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalActiveLogin"
                                    onclick='<%# String.Format("return showActiveLogin(`{0}`, `{1}`);", Eval("Id").ToString(), Convert.ToInt32(Eval("Active"))) %>'
                                    ><%#
                                    TextActive_Login(Eval("Active").ToString())
                                    %></a
                                  >
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalDeleteLogin"
                                    onclick='<%# String.Format("return showDeleteLogin(`{0}`);", Eval("Id").ToString()) %>'
                                    >Delete</a
                                  >
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalResetPass"
                                    onclick='<%# String.Format("return showResetPass(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("UserName").ToString()) %>'
                                    >Reset Password</a
                                  >
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalDencryptPass"
                                    onclick='<%# String.Format("return showDencryptPass(`{0}`, `{1}`);", Eval("UserName").ToString(), DencryptPassword(Eval("Password").ToString())) %>'
                                    >Show Password</a
                                  >
                                  <div class="dropdown-divider"></div>
                                  <asp:LinkButton
                                    runat="server"
                                    CssClass="dropdown-item"
                                    ID="linkLogLogin"
                                    Text="Logs"
                                    CommandName="Log"
                                    CommandArgument='<%# Eval("Id") %>'
                                  ></asp:LinkButton>
                                </div>
                              </ItemTemplate>
                            </asp:TemplateField>
                          </Columns>
                          <PagerStyle
                            BackColor="DodgerBlue"
                            ForeColor="White"
                            HorizontalAlign="Center"
                          />
                          <PagerSettings
                            PreviousPageText="Prev"
                            NextPageText="Next"
                            Mode="NumericFirstLast"
                          />
                          <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                      </div>
                    </div>
                  </div>
                  <div class="row text-start">
                    <div class="col-12">
                      <asp:Button
                        runat="server"
                        ID="btnAddLogin"
                        Text="New Login"
                        CssClass="btn btn-primary"
                        OnClick="btnAddLogin_Click"
                      />
                      <a
                        href="#"
                        runat="server"
                        id="aMailLogin"
                        class="btn btn-info"
                        data-bs-toggle="modal"
                        data-bs-target="#modalMailLogin"
                        >Email Login Details</a
                      >
                    </div>
                  </div>
                </div>

                <div class="tab-pane" id="tabsDiscount">
                  <div class="row mb-3" runat="server" id="divErrorDiscount">
                    <div class="col-12">
                      <div
                        class="alert alert-important alert-danger alert-dismissible"
                        role="alert"
                      >
                        <div class="d-flex">
                          <div>
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              class="icon alert-icon"
                              width="24"
                              height="24"
                              viewBox="0 0 24 24"
                              stroke-width="2"
                              stroke="currentColor"
                              fill="none"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                            >
                              <path
                                stroke="none"
                                d="M0 0h24v24H0z"
                                fill="none"
                              />
                              <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                              <path d="M12 8v4" />
                              <path d="M12 16h.01" />
                            </svg>
                          </div>
                          <div>
                            <span runat="server" id="msgErrorDiscount"></span>
                          </div>
                        </div>
                        <a
                          class="btn-close"
                          data-bs-dismiss="alert"
                          aria-label="close"
                        ></a>
                      </div>
                    </div>
                  </div>
                  <div class="row mb-3">
                    <div class="col-12">
                      <div class="table-responsive">
                        <asp:GridView
                          runat="server"
                          ID="gvListDiscount"
                          CssClass="table table-vcenter card-table"
                          AutoGenerateColumns="false"
                          AllowPaging="true"
                          ShowHeaderWhenEmpty="true"
                          EmptyDataText="DISCOUNT NOT FOUND :)"
                          EmptyDataRowStyle-HorizontalAlign="Center"
                          PageSize="10"
                          PagerSettings-Position="TopAndBottom"
                          OnPageIndexChanging="gvListDiscount_PageIndexChanging"
                          OnRowCommand="gvListDiscount_RowCommand"
                        >
                          <RowStyle />
                          <Columns>
                            <asp:TemplateField
                              HeaderText="#"
                              ItemStyle-Width="60px"
                            >
                              <ItemTemplate>
                                <%# Container.DataItemIndex + 1 %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Id" HeaderText="ID" />
                            <asp:TemplateField HeaderText="Title">
                              <ItemTemplate>
                                <%# TextDiscount(Eval("Id").ToString()) %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Discount">
                              <ItemTemplate>
                                <%# ValueDiscount(Eval("Discount")) %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField
                              DataField="StartDate"
                              HeaderText="Start Date"
                              DataFormatString="{0:dd MMM yyyy}"
                            />
                            <asp:BoundField
                              DataField="EndDate"
                              HeaderText="End Date"
                              DataFormatString="{0:dd MMM yyyy}"
                            />
                            <asp:TemplateField
                              HeaderText="Final Discount (Fabric)"
                            >
                              <ItemTemplate>
                                <%# FinalDiscount(Eval("DiscountType"),
                                Eval("FinalDiscount")) %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120px">
                              <ItemTemplate>
                                <button
                                  class="btn btn-sm btn-pill btn-purple"
                                  data-bs-toggle="dropdown"
                                  runat="server"
                                  visible="<%# VisibleActionDiscount() %>"
                                >
                                  Actions
                                </button>
                                <div class="dropdown-menu dropdown-menu-end">
                                  <asp:LinkButton
                                    runat="server"
                                    ID="linkDetailDiscount"
                                    CssClass="dropdown-item"
                                    Text="Detail / Edit"
                                    CommandName="Detail"
                                    CommandArgument='<%# Eval("Id") %>'
                                  ></asp:LinkButton>
                                  <a
                                    href="#"
                                    runat="server"
                                    class="dropdown-item"
                                    data-bs-toggle="modal"
                                    data-bs-target="#modalDeleteDiscount"
                                    onclick='<%# String.Format("return showDeleteDiscount(`{0}`);", Eval("Id").ToString()) %>'
                                    >Delete</a
                                  >
                                </div>
                              </ItemTemplate>
                            </asp:TemplateField>
                          </Columns>
                          <PagerStyle
                            BackColor="DodgerBlue"
                            ForeColor="White"
                            HorizontalAlign="Center"
                          />
                          <PagerSettings
                            PreviousPageText="Prev"
                            NextPageText="Next"
                            Mode="NumericFirstLast"
                          />
                          <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                      </div>
                    </div>
                  </div>
                  <div class="row text-start">
                    <div class="col-12">
                      <asp:Button
                        runat="server"
                        ID="btnAddDiscount"
                        Text="Add Discount"
                        CssClass="btn btn-primary"
                        OnClick="btnAddDiscount_Click"
                      />
                      <asp:Button
                        runat="server"
                        ID="btnAddCustomDiscount"
                        Text="Add Custom Discount (Fabric)"
                        CssClass="btn btn-secondary"
                        OnClick="btnAddCustomDiscount_Click"
                        hidden
                      />
                    </div>
                  </div>
                </div>

                <div class="tab-pane" id="tabsAccess">
                  <div class="row mb-3" runat="server" id="divErrorProduct">
                    <div class="col-12">
                      <div
                        class="alert alert-important alert-danger alert-dismissible"
                        role="alert"
                      >
                        <div class="d-flex">
                          <div>
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              class="icon alert-icon"
                              width="24"
                              height="24"
                              viewBox="0 0 24 24"
                              stroke-width="2"
                              stroke="currentColor"
                              fill="none"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                            >
                              <path
                                stroke="none"
                                d="M0 0h24v24H0z"
                                fill="none"
                              />
                              <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                              <path d="M12 8v4" />
                              <path d="M12 16h.01" />
                            </svg>
                          </div>
                          <div>
                            <span runat="server" id="msgErrorProduct"></span>
                          </div>
                        </div>
                        <a
                          class="btn-close"
                          data-bs-dismiss="alert"
                          aria-label="close"
                        ></a>
                      </div>
                    </div>
                  </div>

                  <div class="row mb-3">
                    <div class="col-12">
                      <div class="table-responsive">
                        <asp:GridView
                          runat="server"
                          ID="gvListProduct"
                          CssClass="table table-vcenter card-table"
                          AutoGenerateColumns="false"
                          EmptyDataText="DATA NOT FOUND :)"
                          EmptyDataRowStyle-HorizontalAlign="Center"
                          PagerSettings-Position="TopAndBottom"
                        >
                          <RowStyle />
                          <Columns>
                            <asp:BoundField
                              DataField="Id"
                              HeaderStyle-CssClass="hiddencol"
                              ItemStyle-CssClass="hiddencol"
                            />
                            <asp:TemplateField
                              HeaderText="Product"
                              HeaderStyle-HorizontalAlign="Center"
                            >
                              <ItemTemplate>
                                <%# BindDetailProduct(Eval("Id").ToString()) %>
                              </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ItemStyle-Width="120px">
                              <ItemTemplate>
                                <asp:LinkButton
                                  runat="server"
                                  ID="linkEditProduct"
                                  CssClass="btn btn-sm btn-pill btn-purple"
                                  Text="Edit"
                                  OnClick="linkEditProduct_Click"
                                ></asp:LinkButton>
                              </ItemTemplate>
                            </asp:TemplateField>
                          </Columns>
                          <PagerStyle
                            BackColor="DodgerBlue"
                            ForeColor="White"
                            HorizontalAlign="Center"
                          />
                          <PagerSettings
                            PreviousPageText="Prev"
                            NextPageText="Next"
                            Mode="NumericFirstLast"
                          />
                          <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                      </div>
                    </div>
                  </div>

                  <div class="row text-start">
                    <div class="col-12">
                      <asp:Button
                        runat="server"
                        ID="btnAddProduct"
                        Text="New Access"
                        CssClass="btn btn-primary"
                        OnClick="btnAddProduct_Click"
                      />
                    </div>
                  </div>
                </div>

                <div class="tab-pane" id="tabsQuote">
                  <div class="row mb-3" runat="server" id="divErrorQuote">
                    <div class="col-12">
                      <div
                        class="alert alert-important alert-danger alert-dismissible"
                        role="alert"
                      >
                        <div class="d-flex">
                          <div>
                            <svg
                              xmlns="http://www.w3.org/2000/svg"
                              class="icon alert-icon"
                              width="24"
                              height="24"
                              viewBox="0 0 24 24"
                              stroke-width="2"
                              stroke="currentColor"
                              fill="none"
                              stroke-linecap="round"
                              stroke-linejoin="round"
                            >
                              <path
                                stroke="none"
                                d="M0 0h24v24H0z"
                                fill="none"
                              />
                              <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                              <path d="M12 8v4" />
                              <path d="M12 16h.01" />
                            </svg>
                          </div>
                          <div>
                            <span runat="server" id="msgErrorQuote"></span>
                          </div>
                        </div>
                        <a
                          class="btn-close"
                          data-bs-dismiss="alert"
                          aria-label="close"
                        ></a>
                      </div>
                    </div>
                  </div>

                  <div class="row mb-3">
                    <div class="col-12">
                      <div class="table-responsive">
                        <asp:GridView
                          runat="server"
                          ID="gvListQuote"
                          CssClass="table table-vcenter card-table"
                          AutoGenerateColumns="false"
                          AllowPaging="True"
                          EmptyDataText="DATA NOT FOUND :)"
                          EmptyDataRowStyle-HorizontalAlign="Center"
                          PageSize="30"
                          PagerSettings-Position="TopAndBottom"
                        >
                          <RowStyle />
                          <Columns>
                            <asp:BoundField
                              DataField="Id"
                              HeaderStyle-CssClass="hiddencol"
                              ItemStyle-CssClass="hiddencol"
                            />
                            <asp:BoundField
                              DataField="Logo"
                              HeaderText="Logo"
                            />
                            <asp:BoundField
                              DataField="Terms"
                              HeaderText="Terms"
                            />
                            <asp:TemplateField ItemStyle-Width="120px">
                              <ItemTemplate>
                                <asp:LinkButton
                                  runat="server"
                                  ID="linkEditQuote"
                                  CssClass="btn btn-sm btn-pill btn-purple"
                                  Text="Edit"
                                  OnClick="linkEditQuote_Click"
                                ></asp:LinkButton>
                              </ItemTemplate>
                            </asp:TemplateField>
                          </Columns>
                          <PagerStyle
                            BackColor="DodgerBlue"
                            ForeColor="White"
                            HorizontalAlign="Center"
                          />
                          <PagerSettings
                            PreviousPageText="Prev"
                            NextPageText="Next"
                            Mode="NumericFirstLast"
                          />
                          <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                      </div>
                    </div>
                  </div>

                  <div class="row text-start">
                    <div class="col-12">
                      <asp:Button
                        runat="server"
                        ID="btnAddQuote"
                        Text="New Quote"
                        CssClass="btn btn-primary"
                        OnClick="btnAddQuote_Click"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalCreateOrder"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Create Order</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          <div class="mb-4 row">
            <div class="col-12">
              <label class="form-label required">Order Number</label>
              <asp:TextBox
                runat="server"
                ID="txtOrderNumber"
                CssClass="form-control"
                placeholder="Order Number ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-4 row">
            <div class="col-12">
              <label class="form-label required">Customer Name</label>
              <asp:TextBox
                runat="server"
                ID="txtOrderName"
                CssClass="form-control"
                placeholder="Customer Name ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-12">
              <label class="form-label">NOTE</label>
              <asp:TextBox
                runat="server"
                ID="txtOrderNote"
                Height="100px"
                TextMode="MultiLine"
                CssClass="form-control"
                placeholder="Your note for this order ..."
                autocomplete="off"
                style="resize: none"
              ></asp:TextBox>
            </div>
          </div>

          <div class="row" runat="server" id="divErrorCreateOrder">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorCreateOrder"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnSubmitCreateOrder"
            Text="Submit"
            CssClass="btn btn-primary"
            OnClick="btnSubmitCreateOrder_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalDelete"
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
          <h3>Delete Customer</h3>
          <div class="text-secondary">
            You will also delete <b>Customer Contact</b>,
            <b>Customer Address</b>, <b>Customer Login</b>,
            <b>Customer Discount</b> and <b>Customer Product Access</b> data.
          </div>
          <div class="text-secondary">
            <br />
            <span><b>Are you sure you would like to do this?</b></span>
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnSubmitDelete"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnSubmitDelete_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalLog"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title">Changelog</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>

        <div class="modal-body">
          <div class="table-responsive">
            <asp:GridView
              runat="server"
              ID="gvListLogs"
              CssClass="table table-vcenter card-table"
              AutoGenerateColumns="false"
              EmptyDataText="DATA LOG NOT FOUND"
              EmptyDataRowStyle-HorizontalAlign="Center"
              ShowHeader="false"
              GridLines="None"
              BorderStyle="None"
            >
              <RowStyle />
              <Columns>
                <asp:TemplateField>
                  <ItemTemplate>
                    <%# BindTextLog(Eval("Id").ToString()) %>
                  </ItemTemplate>
                </asp:TemplateField>
              </Columns>
              <AlternatingRowStyle BackColor="White" />
            </asp:GridView>
          </div>

          <div class="row" runat="server" id="divErrorLog">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorLog"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <%--CUSTOMER CONTACT--%>

  <div
    class="modal modal-blur fade"
    id="modalProcessContact"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" runat="server" id="titleContact"></h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label required">Name</label>
              <asp:TextBox
                runat="server"
                ID="txtContactName"
                CssClass="form-control"
                placeholder="Name ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
            <div class="col-6">
              <label class="form-label">Salutation</label>
              <asp:DropDownList
                runat="server"
                ID="ddlContactSalutation"
                CssClass="form-select"
              >
                <asp:ListItem Value="" Text=""></asp:ListItem>
                <asp:ListItem Value="Mr." Text="Mr."></asp:ListItem>
                <asp:ListItem Value="Mrs." Text="Mrs."></asp:ListItem>
                <asp:ListItem Value="Ms." Text="Ms."></asp:ListItem>
              </asp:DropDownList>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label">Role</label>
              <asp:TextBox
                runat="server"
                ID="txtContactRole"
                CssClass="form-control"
                placeholder="Role ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
            <div class="col-6">
              <label class="form-label">Email</label>
              <asp:TextBox
                runat="server"
                ID="txtContactEmail"
                TextMode="Email"
                CssClass="form-control"
                placeholder="Email ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label">Phone</label>
              <asp:TextBox
                runat="server"
                ID="txtContactPhone"
                CssClass="form-control"
                placeholder="Phone ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
            <div class="col-6">
              <label class="form-label">Mobile</label>
              <asp:TextBox
                runat="server"
                ID="txtContactMobile"
                CssClass="form-control"
                placeholder="Mobile ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label">FAX</label>
              <asp:TextBox
                runat="server"
                ID="txtContactFax"
                CssClass="form-control"
                placeholder="FAX ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
            <div class="col-6">
              <label class="form-label">Tags</label>
              <asp:ListBox
                runat="server"
                ID="lbContactTags"
                CssClass="form-select"
                SelectionMode="multiple"
                ClientIDMode="Static"
              >
                <asp:ListItem Value="Owner" Text="Owner"></asp:ListItem>
                <asp:ListItem
                  Value="Customer Service"
                  Text="Customer Service"
                ></asp:ListItem>
                <asp:ListItem Value="Pricing" Text="Pricing"></asp:ListItem>
                <asp:ListItem
                  Value="Newsletter"
                  Text="Newsletter"
                ></asp:ListItem>
              </asp:ListBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-12">
              <label class="form-label">Note</label>
              <asp:TextBox
                runat="server"
                TextMode="MultiLine"
                ID="txtContactNote"
                CssClass="form-control"
                Height="100px"
                placeholder="Note ..."
                autocomplete="off"
                style="resize: none"
              ></asp:TextBox>
            </div>
          </div>

          <div class="row" runat="server" id="divErrorProcessContact">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorProcessContact"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnProcessContact"
            Text="Submit"
            CssClass="btn btn-primary"
            OnClick="btnProcessContact_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalDeleteContact"
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
          <h3>Delete Customer Contact</h3>
          <asp:TextBox
            runat="server"
            ID="txtIdContactDelete"
            style="display: none"
          ></asp:TextBox>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnDeleteContact"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnDeleteContact_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalResetPrimaryContact"
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
          <h3>Reset Primary Contact</h3>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnResetPrimaryContact"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnResetPrimaryContact_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalPrimaryContact"
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
          <h3>Set As Primary Contact</h3>
          <asp:TextBox
            runat="server"
            ID="txtIdContactPrimary"
            style="display: none"
          ></asp:TextBox>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnSubmitPrimaryContact"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnSubmitPrimaryContact_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <%--CUSTOMER ADDRESS--%>

  <div
    class="modal modal-blur fade"
    id="modalProcessAddress"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" runat="server" id="titleAddress"></h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>

        <div class="modal-body">
          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label required">Description</label>
              <asp:TextBox
                runat="server"
                ID="txtAddressDescription"
                CssClass="form-control"
                placeholder="Description ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
            <div class="col-6">
              <label class="form-label">Unit Number</label>
              <asp:TextBox
                runat="server"
                ID="txtAddressUnitNumber"
                CssClass="form-control"
                placeholder="Unit Number ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label required">Street Address</label>
              <asp:TextBox
                runat="server"
                ID="txtAddressStreet"
                CssClass="form-control"
                placeholder="Street Address ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
            <div class="col-6">
              <label class="form-label required">Suburb</label>
              <asp:TextBox
                runat="server"
                ID="txtAddressSuburb"
                CssClass="form-control"
                placeholder="Suburb ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-lg-6 col-md-12 col-sm-12">
              <label class="form-label required">State</label>
              <asp:DropDownList
                runat="server"
                ID="ddlAddressStates"
                CssClass="form-select"
              >
                <asp:ListItem Value="" Text=""></asp:ListItem>
                <asp:ListItem Value="ACT" Text="ACT"></asp:ListItem>
                <asp:ListItem Value="NSW" Text="NSW"></asp:ListItem>
                <asp:ListItem Value="NT" Text="NT"></asp:ListItem>
                <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                <asp:ListItem Value="TAS" Text="TAS"></asp:ListItem>
                <asp:ListItem Value="VIC" Text="VIC"></asp:ListItem>
                <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
              </asp:DropDownList>
            </div>
            <div class="col-lg-6 col-md-12 col-sm-12">
              <label class="form-label required">Post Code</label>
              <asp:TextBox
                runat="server"
                ID="txtAddressPostCode"
                CssClass="form-control"
                placeholder="Post Code ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label required">Nearest Port</label>
              <asp:DropDownList
                runat="server"
                ID="ddlAddressPort"
                CssClass="form-select"
              >
                <asp:ListItem Value="" Text=""></asp:ListItem>
                <asp:ListItem Value="Adelaide" Text="Adelaide"></asp:ListItem>
                <asp:ListItem Value="Brisbane" Text="Brisbane"></asp:ListItem>
                <asp:ListItem Value="Melbourne" Text="Melbourne"></asp:ListItem>
                <asp:ListItem Value="Perth" Text="Perth"></asp:ListItem>
                <asp:ListItem Value="Sydney" Text="Sydney"></asp:ListItem>
              </asp:DropDownList>
            </div>
            <div class="col-6">
              <label class="form-label">Tags</label>
              <asp:ListBox
                runat="server"
                ID="lbAddressTags"
                CssClass="form-select"
                SelectionMode="multiple"
                ClientIDMode="Static"
              >
                <asp:ListItem Value="Billing" Text="Billing"></asp:ListItem>
                <asp:ListItem Value="Install" Text="Install"></asp:ListItem>
                <asp:ListItem Value="Delivery" Text="Delivery"></asp:ListItem>
              </asp:ListBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-12">
              <label class="form-label">Instruction</label>
              <asp:TextBox
                runat="server"
                TextMode="MultiLine"
                ID="txtAddressInstruction"
                CssClass="form-control"
                Height="100px"
                placeholder="Instruction ..."
                autocomplete="off"
                style="resize: none"
              ></asp:TextBox>
            </div>
          </div>

          <div class="row" runat="server" id="divErrorProcessAddress">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorProcessAddress"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnProcessAddress"
            Text="Submit"
            CssClass="btn btn-primary"
            OnClick="btnProcessAddress_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalDeleteAddress"
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
          <h3>Delete Customer Address</h3>
          <asp:TextBox
            runat="server"
            ID="txtIdAddressDelete"
            style="display: none"
          ></asp:TextBox>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnDeleteAddress"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnDeleteAddress_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalPrimaryAddress"
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
          <h3>Set As Primary Address</h3>
          <asp:TextBox
            runat="server"
            ID="txtIdAddressPrimary"
            style="display: none"
          ></asp:TextBox>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnSubmitPrimaryAddress"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnSubmitPrimaryAddress_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalResetPrimaryAddress"
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
          <h3>Reset Primary Address</h3>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnResetPrimaryAddress"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnResetPrimaryAddress_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <%--CUSTOMER LOGIN--%>
  <div
    class="modal modal-blur fade"
    id="modalProcessLogin"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" runat="server" id="titleLogin"></h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>

        <div class="modal-body">
          <div class="mb-4 row" runat="server" id="divApplication">
            <div class="col-12">
              <label class="form-label required">Application</label>
              <asp:DropDownList
                runat="server"
                ID="ddlLoginAppId"
                CssClass="form-select"
              ></asp:DropDownList>
            </div>
          </div>

          <div class="mb-4 row" runat="server" id="divAccess">
            <div class="col-6">
              <label class="form-label required">Role</label>
              <asp:DropDownList
                runat="server"
                ID="ddlLoginRole"
                CssClass="form-select"
              ></asp:DropDownList>
            </div>

            <div class="col-6">
              <label class="form-label required">Level</label>
              <asp:DropDownList
                runat="server"
                ID="ddlLoginLevel"
                CssClass="form-select"
              ></asp:DropDownList>
            </div>
          </div>

          <div class="mb-4 row">
            <div class="col-12">
              <label class="form-label required">Full Name</label>
              <asp:TextBox
                runat="server"
                ID="txtLoginFullName"
                CssClass="form-control"
                placeholder="Full Name ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-4 row">
            <div class="col-12">
              <label class="form-label required">Username</label>
              <asp:TextBox
                runat="server"
                ID="txtLoginUserName"
                CssClass="form-control"
                placeholder="UserName ..."
                autocomplete="new-password"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-4 row" runat="server" id="divPassword">
            <div class="col-12">
              <label class="form-label">Password</label>
              <asp:TextBox
                runat="server"
                ID="txtLoginPassword"
                CssClass="form-control"
                placeholder="Password ..."
              ></asp:TextBox>
            </div>
            <small class="form-hint" id="passwordinfo"></small>
          </div>

          <div class="mb-4 row" runat="server" id="divMarkupPrice">
            <div class="col-6">
              <label class="form-label" id="lblMarkup" runat="server"
                >Markup</label
              >
              <asp:DropDownList
                runat="server"
                ID="ddlMarkup"
                CssClass="form-select"
              >
                <asp:ListItem Value="False">NO</asp:ListItem>
                <asp:ListItem Value="True">YES</asp:ListItem>
              </asp:DropDownList>
            </div>
            <div class="col-6">
              <label class="form-label">Price</label>
              <asp:DropDownList
                runat="server"
                ID="ddlPrice"
                CssClass="form-select"
              >
                <asp:ListItem Value="False">NO</asp:ListItem>
                <asp:ListItem Value="True">YES</asp:ListItem>
              </asp:DropDownList>
            </div>
          </div>

          <div class="row" runat="server" id="divErrorProcessLogin">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorProcessLogin"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnProcessLogin"
            Text="Submit"
            CssClass="btn btn-primary"
            OnClick="btnProcessLogin_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalActiveLogin"
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
          <h3 id="hActiveLogin"></h3>
          <asp:TextBox
            runat="server"
            ID="txtIdActiveLogin"
            style="display: none"
          ></asp:TextBox>
          <asp:TextBox
            runat="server"
            ID="txtActiveLogin"
            style="display: none"
          ></asp:TextBox>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnActiveLogin"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnActiveLogin_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalDeleteLogin"
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
          <h3>Delete Customer Login</h3>
          <asp:TextBox
            runat="server"
            ID="txtIdLoginDelete"
            style="display: none"
          ></asp:TextBox>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnDeleteLogin"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnDeleteLogin_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalResetPass"
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
        <div class="modal-status bg-primary"></div>
        <div class="modal-body text-center py-4">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="icon mb-2 text-primary icon-lg"
          >
            <path stroke="none" d="M0 0h24v24H0z" fill="none" />
            <path d="M3.06 13a9 9 0 1 0 .49 -4.087" />
            <path d="M3 4.001v5h5" />
            <path d="M12 12m-1 0a1 1 0 1 0 2 0a1 1 0 1 0 -2 0" />
          </svg>
          <asp:TextBox
            runat="server"
            ID="txtIdResetPass"
            style="display: none"
          ></asp:TextBox>
          <asp:TextBox
            runat="server"
            ID="txtNewResetPass"
            style="display: none"
          ></asp:TextBox>
          <h3>Reset Password</h3>
          <div class="text-secondary">
            <span id="spanDescResetPass"></span>
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnResetPass"
            CssClass="btn btn-primary"
            Text="Confirm"
            OnClick="btnResetPass_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalDencryptPass"
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
        <div class="modal-status"></div>
        <div class="modal-body text-center py-4">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="icon mb-2 text-primary icon-lg"
          >
            <path stroke="none" d="M0 0h24v24H0z" fill="none" />
            <path
              d="M16.555 3.843l3.602 3.602a2.877 2.877 0 0 1 0 4.069l-2.643 2.643a2.877 2.877 0 0 1 -4.069 0l-.301 -.301l-6.558 6.558a2 2 0 0 1 -1.239 .578l-.175 .008h-1.172a1 1 0 0 1 -.993 -.883l-.007 -.117v-1.172a2 2 0 0 1 .467 -1.284l.119 -.13l.414 -.414h2v-2h2v-2l2.144 -2.144l-.301 -.301a2.877 2.877 0 0 1 0 -4.069l2.643 -2.643a2.877 2.877 0 0 1 4.069 0z"
            />
            <path d="M15 9h.01" />
          </svg>
          <h3>Show Password</h3>
          <div class="text-secondary">
            <span id="spanPassword"></span>
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn btn-primary" data-bs-dismiss="modal">Close</a>
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalMailLogin"
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
        <div class="modal-status bg-primary"></div>
        <div class="modal-body text-center py-4">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="24"
            height="24"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            class="icon mb-2 text-primary icon-lg"
          >
            <path stroke="none" d="M0 0h24v24H0z" fill="none" />
            <path d="M3 7h3" />
            <path d="M3 11h2" />
            <path
              d="M9.02 8.801l-.6 6a2 2 0 0 0 1.99 2.199h7.98a2 2 0 0 0 1.99 -1.801l.6 -6a2 2 0 0 0 -1.99 -2.199h-7.98a2 2 0 0 0 -1.99 1.801z"
            />
            <path d="M9.8 7.5l2.982 3.28a3 3 0 0 0 4.238 .202l3.28 -2.982" />
          </svg>
          <h3>Email Login Details</h3>
          <div class="text-secondary">
            Are you sure you want to email this customer the login details?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnMailLogin"
            Text="Confirm"
            CssClass="btn btn-primary"
            OnClick="btnMailLogin_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <%--CUSTOMER DISCOUNT--%>
  <div
    class="modal modal-blur fade"
    id="modalProcessDiscount"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" runat="server" id="titleDiscount"></h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>

        <div class="modal-body">
          <div class="mb-3 row">
            <div class="col-4">
              <label class="form-label required">Discount Type</label>
              <asp:DropDownList
                runat="server"
                ID="ddlDiscountType"
                CssClass="form-select"
                ClientIDMode="Static"
              >
                <asp:ListItem Value="Product" Text="Product"></asp:ListItem>
                <%--
                <asp:ListItem Value="Fabric" Text="Fabric"></asp:ListItem> --%>
              </asp:DropDownList>
            </div>
            <div class="col-8 product">
              <label class="form-label required">Product</label>
              <asp:DropDownList
                runat="server"
                ID="ddlDiscountDesign"
                CssClass="form-select"
              ></asp:DropDownList>
            </div>

            <div class="col-8 fabric">
              <label class="form-label required">Fabric</label>
              <asp:DropDownList
                runat="server"
                ID="ddlDiscountFabric"
                CssClass="form-select"
                ClientIDMode="Static"
              ></asp:DropDownList>
            </div>
          </div>

          <div class="mb-3 row" runat="server" id="divFabricColour">
            <div class="col-12">
              <label class="form-label">Custom Fabric Colour</label>
              <asp:ListBox
                runat="server"
                ID="lbFabricColour"
                CssClass="form-select"
                ClientIDMode="Static"
                SelectionMode="Multiple"
              ></asp:ListBox>
              <small class="form-hint"
                >* Leave this blank, if the discount applies to all fabric
                colors.</small
              >
            </div>
          </div>

          <div class="mb-3 row" runat="server" id="divFabricProduct">
            <div class="col-12">
              <label class="form-label">Custom Fabric Product</label>
              <asp:ListBox
                runat="server"
                ID="lbFabricProduct"
                CssClass="form-select"
                ClientIDMode="Static"
                SelectionMode="Multiple"
              ></asp:ListBox>
            </div>
          </div>

          <div class="mb-3 row fabricSection">
            <div class="col-6">
              <label class="form-label required">Start Date</label>
              <asp:TextBox
                runat="server"
                ID="txtDiscountStart"
                TextMode="Date"
                CssClass="form-control"
                placeholder="From Date ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>

            <div class="col-6">
              <label class="form-label required">End Date</label>
              <asp:TextBox
                runat="server"
                ID="txtDiscountEnd"
                TextMode="Date"
                CssClass="form-control"
                placeholder="To Date ..."
                autocomplete="off"
              ></asp:TextBox>
            </div>
          </div>

          <div class="mb-3 row">
            <div class="col-6">
              <label class="form-label required">Discount</label>
              <div class="input-group">
                <asp:TextBox
                  runat="server"
                  ID="txtDiscountValue"
                  CssClass="form-control"
                  placeholder="Discount ..."
                  autocomplete="off"
                ></asp:TextBox>
                <span class="percent input-group-text">%</span>
              </div>
              <small class="form-hint"
                >* Please use decimal separator with (.)</small
              >
            </div>
            <div class="col-6 fabricSection">
              <label class="form-label">Final Discount</label>
              <asp:DropDownList
                runat="server"
                ID="ddlFinalDiscount"
                CssClass="form-select"
              >
                <asp:ListItem Value="0" Text=""></asp:ListItem>
                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
              </asp:DropDownList>
            </div>
          </div>

          <div class="row" runat="server" id="divErrorProcessDiscount">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorProcessDiscount"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnProcessDiscount"
            Text="Submit"
            CssClass="btn btn-primary"
            OnClick="btnProcessDiscount_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <div
    class="modal modal-blur fade"
    id="modalDeleteDiscount"
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
          <h3>Delete Customer Discount</h3>
          <asp:TextBox
            runat="server"
            ID="txtIdDiscountDelete"
            style="display: none"
          ></asp:TextBox>
          <div class="text-secondary">
            Are you sure you would like to do this?
          </div>
        </div>
        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnDeleteDiscount"
            CssClass="btn btn-danger"
            Text="Confirm"
            OnClick="btnDeleteDiscount_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <%--CUSTOMER ACCESS--%>
  <div
    class="modal modal-blur fade"
    id="modalProcessProduct"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" runat="server" id="titleProduct"></h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          <div class="mb-4 row">
            <div class="col-12">
              <label class="form-label">Tags</label>
              <asp:ListBox
                runat="server"
                ID="lbProductTags"
                CssClass="form-select"
                SelectionMode="multiple"
                ClientIDMode="Static"
              ></asp:ListBox>
            </div>
          </div>

          <div class="row" runat="server" id="divErrorProcessProduct">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorProcessProduct"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnProcessProduct"
            Text="Submit"
            CssClass="btn btn-primary"
            OnClick="btnProcessProduct_Click"
          />
        </div>
      </div>
    </div>
  </div>

  <%--CUSTOMER QUOTE--%>
  <div
    class="modal modal-blur fade"
    id="modalProcessQuote"
    tabindex="-1"
    role="dialog"
    aria-hidden="true"
  >
    <div
      class="modal-dialog modal-dialog-centered modal-dialog-scrollable"
      role="document"
    >
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" runat="server" id="titleQuote"></h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body">
          <div class="row mb-4">
            <div class="col-12">
              <div class="row align-items-center">
                <div class="col-auto">
                  <asp:Image
                    runat="server"
                    ID="imgQuoteLogo"
                    CssClass="d-block"
                    AlternateText="My Logo"
                  />
                </div>
              </div>
            </div>
          </div>

          <h3 class="card-title mt-6" runat="server" id="hLogo">Change Logo</h3>

          <div class="row mb-6">
            <div class="col-12">
              <asp:FileUpload
                runat="server"
                ID="fuLogo"
                CssClass="form-control"
              />
            </div>
          </div>

          <div class="row mb-4">
            <div class="col-12">
              <label class="form-label required">Terms & Conditions</label>
              <asp:TextBox
                runat="server"
                ID="txtQuoteTerms"
                TextMode="MultiLine"
                CssClass="form-control"
                placeholder="Terms & Condition ..."
                autocomplete="off"
                Height="100px"
                style="resize: none"
              ></asp:TextBox>
            </div>
          </div>

          <div class="row" runat="server" id="divErrorProcessQuote">
            <div class="col-12">
              <div
                class="alert alert-important alert-danger alert-dismissible"
                role="alert"
              >
                <div class="d-flex">
                  <div>
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      class="icon alert-icon"
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
                      <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" />
                      <path d="M12 8v4" />
                      <path d="M12 16h.01" />
                    </svg>
                  </div>
                  <div>
                    <span runat="server" id="msgErrorProcessQuote"></span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="modal-footer">
          <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
          <asp:Button
            runat="server"
            ID="btnProccessQuote"
            Text="Submit"
            CssClass="btn btn-primary"
          />
        </div>
      </div>
    </div>
  </div>

  <asp:HiddenField ID="selected_tab" runat="server" />

  <script type="text/javascript">
    document.addEventListener("DOMContentLoaded", () => {
      loaderFadeOut();
    });

    [
      "modalCreateOrder",
      "modalDelete",
      "modalLog",
      "modalProcessContact",
      "modalDeleteContact",
      "modalDeleteContact",
      "modalResetPrimaryContact",
      "modalPrimaryContact",
      "modalProcessAddress",
      "modalDeleteAddress",
      "modalPrimaryAddress",
      "modalResetPrimaryAddress",
      "modalProcessLogin",
      "modalActiveLogin",
      "modalDeleteLogin",
      "modalResetPass",
      "modalDencryptPass",
      "modalMailLogin",
      "modalProcessProduct",
      "modalProcessQuote",
      "modalProcessDiscount",
      "modalDeleteDiscount",
    ].forEach(function (id) {
      document
        .getElementById(id)
        .addEventListener("hide.bs.modal", function () {
          document.activeElement.blur();
          document.body.focus();
        });
    });

    $(document).ready(function () {
      var selectedTab = $("#<%=selected_tab.ClientID%>");
      var tabId = selectedTab.val() != "" ? selectedTab.val() : "tabsContact";
      $('#dvTab a[href="#' + tabId + '"]').tab("show");
      $("#dvTab a").click(function () {
        selectedTab.val($(this).attr("href").substring(1));
      });

      $("#linkContact").on("click", function () {
        updateSessionValue("tabsContact");
      });
      $("#linkAddress").on("click", function () {
        updateSessionValue("tabsAddress");
      });
      $("#linkLogin").on("click", function () {
        updateSessionValue("tabsLogin");
      });
      $("#linkDiscount").on("click", function () {
        updateSessionValue("tabsDiscount");
      });
      $("#linkAccess").on("click", function () {
        updateSessionValue("tabsAccess");
      });
      $("#linkQuote").on("click", function () {
        updateSessionValue("tabsQuote");
      });
    });

    function updateSessionValue(session) {
      $.ajax({
        type: "POST",
        url: "Detail.aspx/UpdateSession",
        data: JSON.stringify({ value: session }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function () {
          //
        },
        error: function (xhr, status, error) {
          console.error("Error updating session:", error);
        },
      });
    }

    function showCreateOrder() {
      $("#modalCreateOrder").modal("show");
    }

    function showLog() {
      $("#modalLog").modal("show");
    }

    //CUSTOMER CONTACT
    function showProcessContact() {
      $("#modalProcessContact").modal("show");
    }

    function showDeleteContact(id) {
      document.getElementById("<%=txtIdContactDelete.ClientID %>").value = id;
    }

    function showPrimaryContact(id) {
      document.getElementById("<%=txtIdContactPrimary.ClientID %>").value = id;
    }

    //CUSTOMER ADDRESS
    function showProcessAddress() {
      $("#modalProcessAddress").modal("show");
    }

    function showDeleteAddress(id) {
      document.getElementById("<%=txtIdAddressDelete.ClientID %>").value = id;
    }

    function showPrimaryAddress(id) {
      document.getElementById("<%=txtIdAddressPrimary.ClientID %>").value = id;
    }

    //CUSTOMER LOGIN
    function showProcessLogin() {
      $("#modalProcessLogin").modal("show");
    }

    function showActiveLogin(id, active) {
      document.getElementById("<%=txtIdActiveLogin.ClientID %>").value = id;
      document.getElementById("<%=txtActiveLogin.ClientID %>").value = active;

      let title = "";
      if (active === 1) {
        title = "Disable Customer Login";
      } else {
        title = "Enable Customer Login";
      }
      document.getElementById("hActiveLogin").innerHTML = title;
    }

    function showDeleteLogin(id) {
      document.getElementById("<%=txtIdLoginDelete.ClientID %>").value = id;
    }

    function showDencryptPass(username, password) {
      let body = "UserName";
      body += "<br />";
      body += "<b>" + username + "</b>";
      body += "<br /><br />";
      body += "Password Decryption";
      body += "<br />";
      body += "<b><u>" + password + "</u></b>";
      document.getElementById("spanPassword").innerHTML = body;
    }

    function showResetPass(id, username) {
      let newPass = generateNewPassword(15);
      let result = `NEW PASSWORD : <br/><b>${newPass}</b><br/><br />Are you sure you want to reset this account password?<br /><br /><b>USERNAME : ${username.toUpperCase()}</b><br /><b>USER ID : ${id.toUpperCase()}</b>`;

      document.getElementById("<%=txtIdResetPass.ClientID %>").value = id;
      document.getElementById("<%=txtNewResetPass.ClientID %>").value = newPass;
      document.getElementById("spanDescResetPass").innerHTML = result;
    }

    function generateNewPassword(length) {
      const chars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
      let result = "";
      const cryptoArray = new Uint8Array(length);
      window.crypto.getRandomValues(cryptoArray);
      for (let i = 0; i < length; i++) {
        result += chars[cryptoArray[i] % chars.length];
      }
      return result;
    }

    //CUSTOMER DISCOUNT

    $("#ddlDiscountType").on("change", function (e) {
      visibleDiscountType();
    });

    function visibleDiscountType() {
      const type = document.getElementById(
        "<%=ddlDiscountType.ClientID %>",
      ).value;
      if (type === "Product") {
        $(".product").show();
        $(".fabric").hide();
        $(".fabricSection").hide();
      } else if (type === "Fabric") {
        $(".product").hide();
        $(".fabric").show();
        $(".fabricSection").show();
      } else {
        $(".product").hide();
        $(".fabric").hide();
        $(".fabricSection").hide();
      }
    }

    function showDeleteDiscount(id) {
      document.getElementById("<%=txtIdDiscountDelete.ClientID %>").value = id;
    }

    function showProcessDiscount() {
      $("#modalProcessDiscount").modal("show");
    }

    //CUSTOMER PRODUCT ACCESS
    function showProcessProduct() {
      $("#modalProcessProduct").modal("show");
    }

    //CUSTOMER QUOTE
    function showProcessQuote() {
      $("#modalProcessQuote").modal("show");
    }

    $(document).on("show.bs.modal", "#modalProcessDiscount", function (e) {
      visibleDiscountType();
    });

    document.addEventListener("DOMContentLoaded", function () {
      function initializeTomSelect(elementId, modalId) {
        var el = document.getElementById(elementId);
        if (window.TomSelect && el) {
          new TomSelect(el, {
            copyClassesToDropdown: false,
            dropdownParent: document.getElementById(modalId),
            controlInput: "<input>",
            render: {
              item: function (data, escape) {
                return data.customProperties
                  ? `<div><span class="dropdown-item-indicator">${data.customProperties}</span>${escape(data.text)}</div>`
                  : `<div>${escape(data.text)}</div>`;
              },
              option: function (data, escape) {
                return data.customProperties
                  ? `<div><span class="dropdown-item-indicator">${data.customProperties}</span>${escape(data.text)}</div>`
                  : `<div>${escape(data.text)}</div>`;
              },
            },
          });
        }
      }

      ["modalProcessDiscount"].forEach(function (id) {
        document
          .getElementById(id)
          .addEventListener("hide.bs.modal", function () {
            document.activeElement.blur();
            document.body.focus();
          });
      });

      initializeTomSelect("lbContactTags", "#modalProcessContact");
      initializeTomSelect("lbAddressTags", "#modalProcessAddress");
      initializeTomSelect("lbProductTags", "#modalProcessProduct");
      initializeTomSelect("lbFabricColour", "#modalProcessDiscount");
      initializeTomSelect("lbFabricProduct", "#modalProcessDiscount");

      var txtLoginUserName = document.getElementById(
        "<%= txtLoginUserName.ClientID %>",
      );
      var passwordInfo = document.getElementById("passwordinfo");

      if (txtLoginUserName) {
        txtLoginUserName.addEventListener("input", function () {
          passwordInfo.innerHTML =
            txtLoginUserName.value.trim() === ""
              ? ""
              : `* If the password input is empty, the password will be <b>${txtLoginUserName.value}</b>`;
        });
      }
    });
  </script>

  <div runat="server" visible="false">
    <asp:Label runat="server" ID="lblIdContact"></asp:Label>
    <asp:Label runat="server" ID="lblActionContact"></asp:Label>

    <asp:Label runat="server" ID="lblIdAddress"></asp:Label>
    <asp:Label runat="server" ID="lblActionAddress"></asp:Label>

    <asp:Label runat="server" ID="lblIdLogin"></asp:Label>
    <asp:Label runat="server" ID="lblLoginUserNameOld"></asp:Label>
    <asp:Label runat="server" ID="lblActionLogin"></asp:Label>

    <asp:Label runat="server" ID="lblIdDiscount"></asp:Label>
    <asp:Label runat="server" ID="lblActionDiscount"></asp:Label>

    <asp:Label runat="server" ID="lblIdProduct"></asp:Label>
    <asp:Label runat="server" ID="lblActionProduct"></asp:Label>

    <asp:Label runat="server" ID="lblIdQuote"></asp:Label>
    <asp:Label runat="server" ID="lblActionQuote"></asp:Label>

    <asp:Label runat="server" ID="lblIdBusiness"></asp:Label>
    <asp:Label runat="server" ID="lblActionBusiness"></asp:Label>
  </div>
</asp:Content>
