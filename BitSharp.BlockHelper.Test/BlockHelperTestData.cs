﻿using BitSharp.BlockHelper;
using BitSharp.Common;
using BitSharp.Common.ExtensionMethods;
using BitSharp.Common.Test;
using BitSharp.WireProtocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitSharp.BlockHelper.Test
{
    public static class BlockHelperTestData
    {
        public const UInt64 SATOSHI_PER_BTC = 100 * 1000 * 1000;

        public static readonly string GENESIS_BLOCK_HASH_STRING = "000000000019d6689c085ae165831e934ff763ae46a2a6c172b3f1b60a8ce26f";

        public static readonly UInt256 GENESIS_BLOCK_HASH = UInt256.Parse(GENESIS_BLOCK_HASH_STRING, NumberStyles.HexNumber);

        public static readonly Block GENESIS_BLOCK = new Block
        (
            Header: new BlockHeader
            (
                Version: 1,
                PreviousBlock: 0,
                MerkleRoot: UInt256.Parse("4a5e1e4baab89f3a32518a88c31bc87f618f76673e2cc77ab2127b7afdeda33b", NumberStyles.HexNumber),
                Time: 1231006505,
                Bits: 486604799,
                Nonce: 2083236893
            ),
            Transactions: ImmutableArray.Create
            (
                new Transaction
                (
                    Version: 1,
                    Inputs: ImmutableArray.Create
                    (
                        new TransactionIn
                        (
                            PreviousTransactionHash: 0,
                            PreviousTransactionIndex: 0xFFFFFFFF,
                            ScriptSignature: ImmutableArray.Create<byte>
                            (
                                0x04, 0xFF, 0xFF, 0x00, 0x1D, 0x01, 0x04, 0x45, 0x54, 0x68, 0x65, 0x20, 0x54, 0x69, 0x6D, 0x65,
                                0x73, 0x20, 0x30, 0x33, 0x2F, 0x4A, 0x61, 0x6E, 0x2F, 0x32, 0x30, 0x30, 0x39, 0x20, 0x43, 0x68,
                                0x61, 0x6E, 0x63, 0x65, 0x6C, 0x6C, 0x6F, 0x72, 0x20, 0x6F, 0x6E, 0x20, 0x62, 0x72, 0x69, 0x6E,
                                0x6B, 0x20, 0x6F, 0x66, 0x20, 0x73, 0x65, 0x63, 0x6F, 0x6E, 0x64, 0x20, 0x62, 0x61, 0x69, 0x6C,
                                0x6F, 0x75, 0x74, 0x20, 0x66, 0x6F, 0x72, 0x20, 0x62, 0x61, 0x6E, 0x6B, 0x73
                            ),
                            Sequence: 0xFFFFFFFF
                        )
                    ),
                    Outputs: ImmutableArray.Create
                    (
                        new TransactionOut
                        (
                            Value: 50 * BlockHelperTestData.SATOSHI_PER_BTC,
                            ScriptPublicKey: ImmutableArray.Create<byte>
                            (
                                0x41, 0x04, 0x67, 0x8A, 0xFD, 0xB0, 0xFE, 0x55, 0x48, 0x27, 0x19, 0x67, 0xF1, 0xA6, 0x71, 0x30,
                                0xB7, 0x10, 0x5C, 0xD6, 0xA8, 0x28, 0xE0, 0x39, 0x09, 0xA6, 0x79, 0x62, 0xE0, 0xEA, 0x1F, 0x61,
                                0xDE, 0xB6, 0x49, 0xF6, 0xBC, 0x3F, 0x4C, 0xEF, 0x38, 0xC4, 0xF3, 0x55, 0x04, 0xE5, 0x1E, 0xC1,
                                0x12, 0xDE, 0x5C, 0x38, 0x4D, 0xF7, 0xBA, 0x0B, 0x8D, 0x57, 0x8A, 0x4C, 0x70, 0x2B, 0x6B, 0xF1,
                                0x1D, 0x5F, 0xAC
                            )
                        )
                    ),
                    LockTime: 0
                )
            )
        );

        public static readonly byte[] GENESIS_BLOCK_BYTES = new byte[]
        {
            0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x3B,0xA3,0xED,0xFD,0x7A,0x7B,0x12,0xB2,0x7A,0xC7,0x2C,0x3E,
            0x67,0x76,0x8F,0x61,0x7F,0xC8,0x1B,0xC3,0x88,0x8A,0x51,0x32,0x3A,0x9F,0xB8,0xAA,
            0x4B,0x1E,0x5E,0x4A,0x29,0xAB,0x5F,0x49,0xFF,0xFF,0x00,0x1D,0x1D,0xAC,0x2B,0x7C,
            0x01,0x01,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
            0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0xFF,0xFF,0x4D,0x04,0xFF,0xFF,0x00,0x1D,
            0x01,0x04,0x45,0x54,0x68,0x65,0x20,0x54,0x69,0x6D,0x65,0x73,0x20,0x30,0x33,0x2F,
            0x4A,0x61,0x6E,0x2F,0x32,0x30,0x30,0x39,0x20,0x43,0x68,0x61,0x6E,0x63,0x65,0x6C,
            0x6C,0x6F,0x72,0x20,0x6F,0x6E,0x20,0x62,0x72,0x69,0x6E,0x6B,0x20,0x6F,0x66,0x20,
            0x73,0x65,0x63,0x6F,0x6E,0x64,0x20,0x62,0x61,0x69,0x6C,0x6F,0x75,0x74,0x20,0x66,
            0x6F,0x72,0x20,0x62,0x61,0x6E,0x6B,0x73,0xFF,0xFF,0xFF,0xFF,0x01,0x00,0xF2,0x05,
            0x2A,0x01,0x00,0x00,0x00,0x43,0x41,0x04,0x67,0x8A,0xFD,0xB0,0xFE,0x55,0x48,0x27,
            0x19,0x67,0xF1,0xA6,0x71,0x30,0xB7,0x10,0x5C,0xD6,0xA8,0x28,0xE0,0x39,0x09,0xA6,
            0x79,0x62,0xE0,0xEA,0x1F,0x61,0xDE,0xB6,0x49,0xF6,0xBC,0x3F,0x4C,0xEF,0x38,0xC4,
            0xF3,0x55,0x04,0xE5,0x1E,0xC1,0x12,0xDE,0x5C,0x38,0x4D,0xF7,0xBA,0x0B,0x8D,0x57,
            0x8A,0x4C,0x70,0x2B,0x6B,0xF1,0x1D,0x5F,0xAC,0x00,0x00,0x00,0x00
        };

        public static readonly byte[] TX_0_HASH_TYPES = new byte[] { 1 };

        public static readonly byte[][] TX_0_SIGNATURES = new byte[][]
        {
            new byte[]
            {
                0x01,0x00,0x00,0x00,0x01,0xC9,0x97,0xA5,0xE5,0x6E,0x10,0x41,0x02,0xFA,0x20,0x9C,0x6A,0x85,0x2D,0xD9,0x06,0x60,0xA2,0x0B,0x2D,0x9C,0x35,
                0x24,0x23,0xED,0xCE,0x25,0x85,0x7F,0xCD,0x37,0x04,0x00,0x00,0x00,0x00,0x43,0x41,0x04,0x11,0xDB,0x93,0xE1,0xDC,0xDB,0x8A,0x01,0x6B,0x49,
                0x84,0x0F,0x8C,0x53,0xBC,0x1E,0xB6,0x8A,0x38,0x2E,0x97,0xB1,0x48,0x2E,0xCA,0xD7,0xB1,0x48,0xA6,0x90,0x9A,0x5C,0xB2,0xE0,0xEA,0xDD,0xFB,
                0x84,0xCC,0xF9,0x74,0x44,0x64,0xF8,0x2E,0x16,0x0B,0xFA,0x9B,0x8B,0x64,0xF9,0xD4,0xC0,0x3F,0x99,0x9B,0x86,0x43,0xF6,0x56,0xB4,0x12,0xA3,
                0xAC,0xFF,0xFF,0xFF,0xFF,0x02,0x00,0xCA,0x9A,0x3B,0x00,0x00,0x00,0x00,0x43,0x41,0x04,0xAE,0x1A,0x62,0xFE,0x09,0xC5,0xF5,0x1B,0x13,0x90,
                0x5F,0x07,0xF0,0x6B,0x99,0xA2,0xF7,0x15,0x9B,0x22,0x25,0xF3,0x74,0xCD,0x37,0x8D,0x71,0x30,0x2F,0xA2,0x84,0x14,0xE7,0xAA,0xB3,0x73,0x97,
                0xF5,0x54,0xA7,0xDF,0x5F,0x14,0x2C,0x21,0xC1,0xB7,0x30,0x3B,0x8A,0x06,0x26,0xF1,0xBA,0xDE,0xD5,0xC7,0x2A,0x70,0x4F,0x7E,0x6C,0xD8,0x4C,
                0xAC,0x00,0x28,0x6B,0xEE,0x00,0x00,0x00,0x00,0x43,0x41,0x04,0x11,0xDB,0x93,0xE1,0xDC,0xDB,0x8A,0x01,0x6B,0x49,0x84,0x0F,0x8C,0x53,0xBC,
                0x1E,0xB6,0x8A,0x38,0x2E,0x97,0xB1,0x48,0x2E,0xCA,0xD7,0xB1,0x48,0xA6,0x90,0x9A,0x5C,0xB2,0xE0,0xEA,0xDD,0xFB,0x84,0xCC,0xF9,0x74,0x44,
                0x64,0xF8,0x2E,0x16,0x0B,0xFA,0x9B,0x8B,0x64,0xF9,0xD4,0xC0,0x3F,0x99,0x9B,0x86,0x43,0xF6,0x56,0xB4,0x12,0xA3,0xAC,0x00,0x00,0x00,0x00,
                0x01,0x00,0x00,0x00
            }
        };

        public static readonly byte[][] TX_0_SIGNATURE_HASHES = new byte[][]
        {
            new byte[]
            {
                0x7a,0x05,0xc6,0x14,0x5f,0x10,0x10,0x1e,0x9d,0x63,0x25,0x49,0x42,0x45,0xad,0xf1,0x29,0x7d,0x80,0xf8,0xf3,0x8d,0x4d,0x57,0x6d,0x57,0xcd,0xba,0x22,0x0b,0xcb,0x19
            }
        };

        public static readonly byte[][] TX_0_X = new byte[][]
        {
            new byte[]
            {
                0x11,0xdb,0x93,0xe1,0xdc,0xdb,0x8a,0x01,0x6b,0x49,0x84,0x0f,0x8c,0x53,0xbc,0x1e,0xb6,0x8a,0x38,0x2e,0x97,0xb1,0x48,0x2e,0xca,0xd7,0xb1,0x48,0xa6,0x90,0x9a,0x5c
            }
        };

        public static readonly byte[][] TX_0_Y = new byte[][]
        {
            new byte[]
            {
                0xb2,0xe0,0xea,0xdd,0xfb,0x84,0xcc,0xf9,0x74,0x44,0x64,0xf8,0x2e,0x16,0x0b,0xfa,0x9b,0x8b,0x64,0xf9,0xd4,0xc0,0x3f,0x99,0x9b,0x86,0x43,0xf6,0x56,0xb4,0x12,0xa3
            }
        };

        public static readonly byte[][] TX_0_R = new byte[][]
        {
            new byte[]
            {
                0x4e,0x45,0xe1,0x69,0x32,0xb8,0xaf,0x51,0x49,0x61,0xa1,0xd3,0xa1,0xa2,0x5f,0xdf,0x3f,0x4f,0x77,0x32,0xe9,0xd6,0x24,0xc6,0xc6,0x15,0x48,0xab,0x5f,0xb8,0xcd,0x41
            }
        };

        public static readonly byte[][] TX_0_S = new byte[][]
        {
            new byte[]
            {
                0x18,0x15,0x22,0xec,0x8e,0xca,0x07,0xde,0x48,0x60,0xa4,0xac,0xdd,0x12,0x90,0x9d,0x83,0x1c,0xc5,0x6c,0xbb,0xac,0x46,0x22,0x08,0x22,0x21,0xa8,0x76,0x8d,0x1d,0x09
            }
        };

        public static readonly byte[] TX_0_MULTI_INPUT_HASH_TYPES = new byte[] { 1, 1, 1 };

        public static readonly byte[][] TX_0_MULTI_INPUT_SIGNATURES = new byte[][]
        {
            new byte[]
            {
                0x01,0x00,0x00,0x00,0x03,0x21,0xf7,0x5f,0x31,0x39,0xa0,0x13,0xf5,0x0f,0x31,0x5b,0x23,0xb0,0xc9,0xa2,0xb6,0xea,0xc3,0x1e,0x2b,0xec,0x98,
                0xe5,0x89,0x1c,0x92,0x46,0x64,0x88,0x99,0x42,0x26,0x00,0x00,0x00,0x00,0x43,0x41,0x04,0x4c,0xa7,0xba,0xf6,0xd8,0xb6,0x58,0xab,0xd0,0x42,
                0x23,0x90,0x9d,0x82,0xf1,0x76,0x47,0x40,0xbd,0xc9,0x31,0x72,0x55,0xf5,0x4e,0x49,0x10,0xf8,0x88,0xbd,0x82,0x95,0x0e,0x33,0x23,0x67,0x98,
                0x51,0x75,0x91,0xe4,0xc2,0x18,0x1f,0x69,0xb5,0xea,0xa2,0xfa,0x1f,0x21,0x86,0x67,0x80,0xa0,0xcc,0x5d,0x83,0x96,0xa0,0x4f,0xd3,0x63,0x10,
                0xac,0xff,0xff,0xff,0xff,0x79,0xcd,0xa0,0x94,0x59,0x03,0x62,0x7c,0x3d,0xa1,0xf8,0x5f,0xc9,0x5d,0x0b,0x8e,0xe3,0xe7,0x6a,0xe0,0xcf,0xdc,
                0x9a,0x65,0xd0,0x97,0x44,0xb1,0xf8,0xfc,0x85,0x43,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0xff,0xfe,0x09,0xf5,0xfe,0x3f,0xfb,0xf5,0xee,
                0x97,0xa5,0x4e,0xb5,0xe5,0x06,0x9e,0x9d,0xa6,0xb4,0x85,0x6e,0xe8,0x6f,0xc5,0x29,0x38,0xc2,0xf9,0x79,0xb0,0xf3,0x8e,0x82,0x00,0x00,0x00,
                0x00,0x00,0xff,0xff,0xff,0xff,0x01,0x00,0x9d,0x96,0x6b,0x01,0x00,0x00,0x00,0x43,0x41,0x04,0xea,0x1f,0xef,0xf8,0x61,0xb5,0x1f,0xe3,0xf5,
                0xf8,0xa3,0xb1,0x2d,0x0f,0x47,0x12,0xdb,0x80,0xe9,0x19,0x54,0x8a,0x80,0x83,0x9f,0xc4,0x7c,0x6a,0x21,0xe6,0x6d,0x95,0x7e,0x9c,0x5d,0x8c,
                0xd1,0x08,0xc7,0xa2,0xd2,0x32,0x4b,0xad,0x71,0xf9,0x90,0x4a,0xc0,0xae,0x73,0x36,0x50,0x7d,0x78,0x5b,0x17,0xa2,0xc1,0x15,0xe4,0x27,0xa3,
                0x2f,0xac,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00
            },
            new byte[]
            {
                0x01,0x00,0x00,0x00,0x03,0x21,0xf7,0x5f,0x31,0x39,0xa0,0x13,0xf5,0x0f,0x31,0x5b,0x23,0xb0,0xc9,0xa2,0xb6,0xea,0xc3,0x1e,0x2b,0xec,0x98,
                0xe5,0x89,0x1c,0x92,0x46,0x64,0x88,0x99,0x42,0x26,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0xff,0x79,0xcd,0xa0,0x94,0x59,0x03,0x62,0x7c,
                0x3d,0xa1,0xf8,0x5f,0xc9,0x5d,0x0b,0x8e,0xe3,0xe7,0x6a,0xe0,0xcf,0xdc,0x9a,0x65,0xd0,0x97,0x44,0xb1,0xf8,0xfc,0x85,0x43,0x00,0x00,0x00,
                0x00,0x43,0x41,0x04,0xfe,0x1b,0x9c,0xcf,0x73,0x2e,0x1f,0x6b,0x76,0x0c,0x5e,0xd3,0x15,0x23,0x88,0xee,0xea,0xdd,0x4a,0x07,0x3e,0x62,0x1f,
                0x74,0x1e,0xb1,0x57,0xe6,0xa6,0x2e,0x35,0x47,0xc8,0xe9,0x39,0xab,0xbd,0x6a,0x51,0x3b,0xf3,0xa1,0xfb,0xe2,0x8f,0x9e,0xa8,0x5a,0x4e,0x64,
                0xc5,0x26,0x70,0x24,0x35,0xd7,0x26,0xf7,0xff,0x14,0xda,0x40,0xba,0xe4,0xac,0xff,0xff,0xff,0xff,0xfe,0x09,0xf5,0xfe,0x3f,0xfb,0xf5,0xee,
                0x97,0xa5,0x4e,0xb5,0xe5,0x06,0x9e,0x9d,0xa6,0xb4,0x85,0x6e,0xe8,0x6f,0xc5,0x29,0x38,0xc2,0xf9,0x79,0xb0,0xf3,0x8e,0x82,0x00,0x00,0x00,
                0x00,0x00,0xff,0xff,0xff,0xff,0x01,0x00,0x9d,0x96,0x6b,0x01,0x00,0x00,0x00,0x43,0x41,0x04,0xea,0x1f,0xef,0xf8,0x61,0xb5,0x1f,0xe3,0xf5,
                0xf8,0xa3,0xb1,0x2d,0x0f,0x47,0x12,0xdb,0x80,0xe9,0x19,0x54,0x8a,0x80,0x83,0x9f,0xc4,0x7c,0x6a,0x21,0xe6,0x6d,0x95,0x7e,0x9c,0x5d,0x8c,
                0xd1,0x08,0xc7,0xa2,0xd2,0x32,0x4b,0xad,0x71,0xf9,0x90,0x4a,0xc0,0xae,0x73,0x36,0x50,0x7d,0x78,0x5b,0x17,0xa2,0xc1,0x15,0xe4,0x27,0xa3,
                0x2f,0xac,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00
            },
            new byte[]
            {
                0x01,0x00,0x00,0x00,0x03,0x21,0xf7,0x5f,0x31,0x39,0xa0,0x13,0xf5,0x0f,0x31,0x5b,0x23,0xb0,0xc9,0xa2,0xb6,0xea,0xc3,0x1e,0x2b,0xec,0x98,
                0xe5,0x89,0x1c,0x92,0x46,0x64,0x88,0x99,0x42,0x26,0x00,0x00,0x00,0x00,0x00,0xff,0xff,0xff,0xff,0x79,0xcd,0xa0,0x94,0x59,0x03,0x62,0x7c,
                0x3d,0xa1,0xf8,0x5f,0xc9,0x5d,0x0b,0x8e,0xe3,0xe7,0x6a,0xe0,0xcf,0xdc,0x9a,0x65,0xd0,0x97,0x44,0xb1,0xf8,0xfc,0x85,0x43,0x00,0x00,0x00,
                0x00,0x00,0xff,0xff,0xff,0xff,0xfe,0x09,0xf5,0xfe,0x3f,0xfb,0xf5,0xee,0x97,0xa5,0x4e,0xb5,0xe5,0x06,0x9e,0x9d,0xa6,0xb4,0x85,0x6e,0xe8,
                0x6f,0xc5,0x29,0x38,0xc2,0xf9,0x79,0xb0,0xf3,0x8e,0x82,0x00,0x00,0x00,0x00,0x43,0x41,0x04,0xbe,0xd8,0x27,0xd3,0x74,0x74,0xbe,0xff,0xb3,
                0x7e,0xfe,0x53,0x37,0x01,0xac,0x1f,0x7c,0x60,0x09,0x57,0xa4,0x48,0x7b,0xe8,0xb3,0x71,0x34,0x6f,0x01,0x68,0x26,0xee,0x6f,0x57,0xba,0x30,
                0xd8,0x8a,0x47,0x2a,0x0e,0x4e,0xcd,0x2f,0x07,0x59,0x9a,0x79,0x5f,0x1f,0x01,0xde,0x78,0xd7,0x91,0xb3,0x82,0xe6,0x5e,0xe1,0xc5,0x8b,0x45,
                0x08,0xac,0xff,0xff,0xff,0xff,0x01,0x00,0x9d,0x96,0x6b,0x01,0x00,0x00,0x00,0x43,0x41,0x04,0xea,0x1f,0xef,0xf8,0x61,0xb5,0x1f,0xe3,0xf5,
                0xf8,0xa3,0xb1,0x2d,0x0f,0x47,0x12,0xdb,0x80,0xe9,0x19,0x54,0x8a,0x80,0x83,0x9f,0xc4,0x7c,0x6a,0x21,0xe6,0x6d,0x95,0x7e,0x9c,0x5d,0x8c,
                0xd1,0x08,0xc7,0xa2,0xd2,0x32,0x4b,0xad,0x71,0xf9,0x90,0x4a,0xc0,0xae,0x73,0x36,0x50,0x7d,0x78,0x5b,0x17,0xa2,0xc1,0x15,0xe4,0x27,0xa3,
                0x2f,0xac,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00
            }
        };

        public static readonly byte[][] TX_0_MULTI_INPUT_SIGNATURE_HASHES = new byte[][]
        {
            new byte[]
            {
                0x21,0x3d,0x14,0x58,0x31,0xc9,0xc9,0x76,0xf9,0x9d,0x03,0x83,0x40,0x44,0x31,0xf7,0x55,0x6e,0x73,0x29,0xdf,0xa3,0x4e,0x79,0x35,0x48,0x63,0xff,0xc7,0x08,0x4f,0x53
            },
            new byte[]
            {
                0x90,0xac,0x67,0xb0,0xe9,0xcd,0x3f,0xe9,0x53,0x89,0x6d,0xd3,0x71,0xff,0x97,0x1e,0x57,0x3a,0x77,0x74,0xd1,0xe5,0xef,0xc1,0x0c,0x62,0x9a,0x41,0x3d,0x1c,0xf9,0x2e
            },
            new byte[]
            {
                0x3e,0xa1,0xf9,0x07,0x24,0x9c,0x4f,0x80,0xf7,0x98,0x82,0x3d,0xe3,0xae,0x5e,0x8d,0x05,0xea,0xf7,0xa2,0x26,0xd4,0x0f,0x30,0xac,0xe7,0xc5,0x4e,0xd9,0x1b,0x76,0x10
            }
        };

        public static readonly byte[][] TX_0_MULTI_INPUT_X = new byte[][]
        {
            new byte[]
            {
                0x4c,0xa7,0xba,0xf6,0xd8,0xb6,0x58,0xab,0xd0,0x42,0x23,0x90,0x9d,0x82,0xf1,0x76,0x47,0x40,0xbd,0xc9,0x31,0x72,0x55,0xf5,0x4e,0x49,0x10,0xf8,0x88,0xbd,0x82,0x95
            },
            new byte[]
            {
                0xfe,0x1b,0x9c,0xcf,0x73,0x2e,0x1f,0x6b,0x76,0x0c,0x5e,0xd3,0x15,0x23,0x88,0xee,0xea,0xdd,0x4a,0x07,0x3e,0x62,0x1f,0x74,0x1e,0xb1,0x57,0xe6,0xa6,0x2e,0x35,0x47
            },
            new byte[]
            {
                0xbe,0xd8,0x27,0xd3,0x74,0x74,0xbe,0xff,0xb3,0x7e,0xfe,0x53,0x37,0x01,0xac,0x1f,0x7c,0x60,0x09,0x57,0xa4,0x48,0x7b,0xe8,0xb3,0x71,0x34,0x6f,0x01,0x68,0x26,0xee
            }
        };

        public static readonly byte[][] TX_0_MULTI_INPUT_Y = new byte[][]
        {
            new byte[]
            {
                0x0e,0x33,0x23,0x67,0x98,0x51,0x75,0x91,0xe4,0xc2,0x18,0x1f,0x69,0xb5,0xea,0xa2,0xfa,0x1f,0x21,0x86,0x67,0x80,0xa0,0xcc,0x5d,0x83,0x96,0xa0,0x4f,0xd3,0x63,0x10
            },
            new byte[]
            {
                0xc8,0xe9,0x39,0xab,0xbd,0x6a,0x51,0x3b,0xf3,0xa1,0xfb,0xe2,0x8f,0x9e,0xa8,0x5a,0x4e,0x64,0xc5,0x26,0x70,0x24,0x35,0xd7,0x26,0xf7,0xff,0x14,0xda,0x40,0xba,0xe4
            },
            new byte[]
            {
                0x6f,0x57,0xba,0x30,0xd8,0x8a,0x47,0x2a,0x0e,0x4e,0xcd,0x2f,0x07,0x59,0x9a,0x79,0x5f,0x1f,0x01,0xde,0x78,0xd7,0x91,0xb3,0x82,0xe6,0x5e,0xe1,0xc5,0x8b,0x45,0x08
            }
        };

        public static readonly byte[][] TX_0_MULTI_INPUT_R = new byte[][]
        {
            new byte[]
            {
                0xcb,0x2c,0x6b,0x34,0x6a,0x97,0x8a,0xb8,0xc6,0x1b,0x18,0xb5,0xe9,0x39,0x77,0x55,0xcb,0xd1,0x7d,0x6e,0xb2,0xfe,0x00,0x83,0xef,0x32,0xe0,0x67,0xfa,0x6c,0x78,0x5a
            },
            new byte[]
            {
                0x47,0x95,0x7c,0xdd,0x95,0x7c,0xfd,0x0b,0xec,0xd6,0x42,0xf6,0xb8,0x4d,0x82,0xf4,0x9b,0x6c,0xb4,0xc5,0x1a,0x91,0xf4,0x92,0x46,0x90,0x8a,0xf7,0xc3,0xcf,0xdf,0x4a
            },
            new byte[]
            {
                0x41,0x65,0xbe,0x9a,0x4c,0xba,0xb8,0x04,0x9e,0x1a,0xf9,0x72,0x3b,0x96,0x19,0x9b,0xfd,0x3e,0x85,0xf4,0x4c,0x6b,0x4c,0x01,0x77,0xe3,0x96,0x26,0x86,0xb2,0x60,0x73
            }
        };

        public static readonly byte[][] TX_0_MULTI_INPUT_S = new byte[][]
        {
            new byte[]
            {
                0x6c,0xe4,0x4e,0x61,0x3f,0x31,0xd9,0xa6,0xb0,0x51,0x7e,0x46,0xf3,0xdb,0x15,0x76,0xe9,0x81,0x2c,0xc9,0x8d,0x15,0x9b,0xfd,0xaf,0x75,0x9a,0x50,0x14,0x08,0x1b,0x5c
            },
            new byte[]
            {
                0xe9,0x6b,0x46,0x62,0x1f,0x1b,0xff,0xcf,0x5e,0xa5,0x98,0x2f,0x88,0xce,0xf6,0x51,0xe9,0x35,0x4f,0x57,0x91,0x60,0x23,0x69,0xbf,0x5a,0x82,0xa6,0xcd,0x61,0xa6,0x25
            },
            new byte[]
            {
                0x28,0xf6,0x38,0xda,0x23,0xfc,0x00,0x37,0x60,0x86,0x1a,0xd4,0x81,0xea,0xd4,0x09,0x93,0x12,0xc6,0x00,0x30,0xd4,0xcb,0x57,0x82,0x0c,0xe4,0xd3,0x38,0x12,0xa5,0xce
            }
        };

        public static readonly byte[] TX_0_HASH160_HASH_TYPES = new byte[] { 1 };

        public static readonly byte[][] TX_0_HASH160_SIGNATURES = new byte[][]
        {
            new byte[]
            {
                0x01,0x00,0x00,0x00,0x01,0x94,0x4b,0xad,0xc3,0x3f,0x9a,0x72,0x3e,0xb1,0xc8,0x5d,0xde,0x24,0x37,0x4e,0x6d,0xee,0x92,0x59,0xef,0x4c,0xfa,
                0x6a,0x10,0xb2,0xfd,0x05,0xb6,0xe5,0x5b,0xe4,0x00,0x00,0x00,0x00,0x00,0x19,0x76,0xa9,0x14,0x69,0x34,0xef,0xce,0xf3,0x69,0x03,0xb5,0xb4,
                0x5e,0xbd,0x1e,0x5f,0x86,0x2d,0x1b,0x63,0xa9,0x9f,0xa5,0x88,0xac,0xff,0xff,0xff,0xff,0x01,0x00,0xf2,0x05,0x2a,0x01,0x00,0x00,0x00,0x19,
                0x76,0xa9,0x14,0x69,0x34,0xef,0xce,0xf3,0x69,0x03,0xb5,0xb4,0x5e,0xbd,0x1e,0x5f,0x86,0x2d,0x1b,0x63,0xa9,0x9f,0xa5,0x88,0xac,0x00,0x00,
                0x00,0x00,0x01,0x00,0x00,0x00
            }
        };

        public static readonly byte[][] TX_0_HASH160_SIGNATURE_HASHES = new byte[][]
        {
            new byte[]
            {
                0x36,0xb6,0xbe,0xc3,0x4f,0xa8,0x43,0xc2,0xf4,0x2a,0xec,0x47,0x2d,0x4e,0x60,0x33,0x99,0xc5,0xe6,0x39,0x08,0x5b,0x79,0x79,0x6f,0x2d,0x3a,0x8a,0x56,0x01,0x7b,0xad
            }
        };

        public static readonly byte[][] TX_0_HASH160_X = new byte[][]
        {
            new byte[]
            {
                0xf9,0x80,0x4c,0xfb,0x86,0xfb,0x17,0x44,0x1a,0x65,0x62,0xb0,0x7c,0x4e,0xe8,0xf0,0x12,0xbd,0xb2,0xda,0x5b,0xe0,0x22,0x03,0x2e,0x4b,0x87,0x10,0x03,0x50,0xcc,0xc7
            }
        };

        public static readonly byte[][] TX_0_HASH160_Y = new byte[][]
        {
            new byte[]
            {
                0xc0,0xf4,0xd4,0x70,0x78,0xb0,0x6c,0x9d,0x22,0xb0,0xec,0x10,0xbd,0xce,0x4c,0x59,0x0e,0x0d,0x01,0xae,0xd6,0x18,0x98,0x7a,0x6c,0xaa,0x8c,0x94,0xd7,0x4e,0xe6,0xdc
            }
        };

        public static readonly byte[][] TX_0_HASH160_R = new byte[][]
        {
            new byte[]
            {
                0x9f,0x8a,0xef,0x83,0x48,0x9d,0x5c,0x35,0x24,0xb6,0x8d,0xdf,0x77,0xe8,0xaf,0x8c,0xeb,0x5c,0xba,0x89,0x79,0x0d,0x31,0xd2,0xd2,0xdb,0x0c,0x80,0xb9,0xcb,0xfd,0x26
            }
        };

        public static readonly byte[][] TX_0_HASH160_S = new byte[][]
        {
            new byte[]
            {
                0xbb,0x2c,0x13,0xe1,0x5b,0xb3,0x56,0xa4,0xac,0xcd,0xd5,0x52,0x88,0xe8,0xb2,0xfd,0x39,0xe2,0x04,0xa9,0x3d,0x84,0x9c,0xcf,0x74,0x9e,0xae,0xf9,0xd8,0x16,0x27,0x87
            }
        };

        public static void GetFirstTransaction(BlockProvider blockProvider, out Block block, out Transaction tx, out IDictionary<UInt256, Transaction> txLookup)
        {
            txLookup = new Dictionary<UInt256, Transaction>();

            // prior outputs for first transaction
            GetTransaction(blockProvider, 9, 0, out block, out tx);
            txLookup.Add(tx.Hash, tx);

            // first transaction
            // do this last so its output is what is returned
            GetTransaction(blockProvider, 170, 1, out block, out tx);
            txLookup.Add(tx.Hash, tx);
        }

        public static void GetFirstMultiInputTransaction(BlockProvider blockProvider, out Block block, out Transaction tx, out IDictionary<UInt256, Transaction> txLookup)
        {
            txLookup = new Dictionary<UInt256, Transaction>();

            // prior outputs for first transaction
            GetTransaction(blockProvider, 360, 0, out block, out tx);
            txLookup.Add(tx.Hash, tx);

            GetTransaction(blockProvider, 187, 1, out block, out tx);
            txLookup.Add(tx.Hash, tx);

            GetTransaction(blockProvider, 248, 1, out block, out tx);
            txLookup.Add(tx.Hash, tx);

            // first transaction
            // do this last so its output is what is returned
            GetTransaction(blockProvider, 496, 1, out block, out tx);
            txLookup.Add(tx.Hash, tx);
        }

        public static void GetFirstHash160Transaction(BlockProvider blockProvider, out Block block, out Transaction tx, out IDictionary<UInt256, Transaction> txLookup)
        {
            txLookup = new Dictionary<UInt256, Transaction>();

            // prior outputs for first OP_HASH160 transaction
            GetTransaction(blockProvider, 2676, 0, out block, out tx);
            txLookup.Add(tx.Hash, tx);

            GetTransaction(blockProvider, 2812, 1, out block, out tx);
            txLookup.Add(tx.Hash, tx);

            // first OP_HASH160 transaction
            // do this last so its output is what is returned
            GetTransaction(blockProvider, 2812, 2, out block, out tx);
            txLookup.Add(tx.Hash, tx);
        }

        public static void GetTransaction(BlockProvider blockProvider, int blockIndex, int txIndex, out Block block, out Transaction tx)
        {
            block = blockProvider.GetBlock(blockIndex);
            tx = block.Transactions[txIndex];
        }
    }
}
