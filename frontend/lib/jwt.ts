import { jwtVerify, JWTPayload } from "jose";

const secretKey =
  process.env.ASP_NET_JWT_SECRET || "HelloMyNameIsMeshaalAndThisIsTutorialForJWT";
const encodedKey = new TextEncoder().encode(secretKey);

export async function decrypt(token?: string): Promise<JWTPayload | null> {
  if (!token) return null;
  try {
    const verifyOptions: {
      issuer?: string;
      audience?: string;
    } = {};

    if (process.env.ASP_NET_JWT_ISSUER) {
      verifyOptions.issuer = process.env.ASP_NET_JWT_ISSUER;
    }
    if (process.env.ASP_NET_JWT_AUDIENCE) {
      verifyOptions.audience = process.env.ASP_NET_JWT_AUDIENCE;
    }

    const { payload } = await jwtVerify(token, encodedKey, verifyOptions);
    return payload;
  } catch (error) {
    return null;
  }
}
