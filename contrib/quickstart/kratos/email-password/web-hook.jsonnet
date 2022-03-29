function(ctx) {
  company: ctx.identity.traits.company,
  externalIdentityId: ctx.identity.id,
  companyUrl: ctx.identity.traits.companyUrl,
  phone: ctx.identity.traits.phone,
  token: "tl-ory-secret-token-abe456cd-9ef2-4205-90a5-d0ad037d7a69",
  fullName: ctx.identity.traits.fullName
}