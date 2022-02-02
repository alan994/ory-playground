local claims = {
  email_verified: true
} + std.extVar('claims');

{
  identity: {
    traits: {
      [if "email" in claims && claims.email_verified then "email" else null]: claims.email,
      // additional claims
      // please also see the `Google specific claims` section
      fullName: claims.name
    },
  },
}